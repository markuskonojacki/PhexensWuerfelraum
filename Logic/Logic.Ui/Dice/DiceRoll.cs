using Dice;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Linq;
using static PhexensWuerfelraum.Logic.Ui.CharacterModel;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class DiceRoll
    {
        private CharacterModel Character { get; set; } = SimpleIoc.Default.GetInstance<CharacterViewModel>().Character;

        public string RollTrial(Talent talent)
        {
            return RollTrial(talent.Name, talent.Probe, talent.Value, talent.Behinderungsfaktor);
        }

        public string RollTrial(Zauber zauber)
        {
            return RollTrial(zauber.Name, zauber.Probe, zauber.Value);
        }

        /// <summary>
        /// Rolls some dice and creates a string from the outcome
        /// </summary>
        /// <param name="trialName">name of the trial</param>
        /// <param name="trial"></param>
        /// <param name="trialValue">TaW/ZfP</param>
        /// <param name="behinderungsfaktorText"></param>
        /// <returns>returns a string that represents the trial outcome</returns>
        public string RollTrial(string trialName, Probe trial, int trialValue, string behinderungsfaktorText = "")
        {
            RollResult rollResult = Roller.Roll("3d20");
            int attribute1Value, attribute2Value, attribute3Value;
            int roll1 = 0, roll2 = 0, roll3 = 0;
            int trialPointsRemaining = trialValue;
            int rolledOnes = 0, rolledTwenties = 0;
            int trialModificationMax = 0;
            string trialModificationTxt = "";
            string beText = "";
            string blindText = "";
            string resultText = "";
            int i = 1;

            foreach (var roll in rollResult.Values)
            {
                if (roll.DieType == DieType.Normal)
                {
                    if (i == 1)
                    {
                        roll1 = Convert.ToInt32(roll.Value);
                    }
                    else if (i == 2)
                    {
                        roll2 = Convert.ToInt32(roll.Value);
                    }
                    else if (i == 3)
                    {
                        roll3 = Convert.ToInt32(roll.Value);
                    }
                    i++;
                }
            }

            if (trial.Attribut1 == AttributType.Wildcard || trial.Attribut2 == AttributType.Wildcard || trial.Attribut3 == AttributType.Wildcard)
            {
                if (Character.RollModeOpen == false)
                    blindText = "(blind) ";

                return string.Format($"{blindText}{trialName}: {roll1}, {roll2}, {roll3} (Probe enthält frei wählbare Attribute)");
            }

            attribute1Value = GetAttributeValue(trial.Attribut1);
            attribute2Value = GetAttributeValue(trial.Attribut2);
            attribute3Value = GetAttributeValue(trial.Attribut3);

            #region Erschwernis / Erleichterung

            int trialModification = Character.Modifikation;
            if (trialModification != 0)
            {
                trialPointsRemaining -= trialModification;

                if (trialModification > 0)
                {
                    trialModificationTxt = string.Format(" [Erschwernis von {0}]", trialModification);
                }
                else
                {
                    trialModificationTxt = string.Format(" [Erleichterung von {0}]", trialModification * -1);
                }
            }

            Character.Modifikation = 0;

            #endregion Erschwernis / Erleichterung

            #region Behinderung / BE

            int be;

            switch (behinderungsfaktorText)
            {
                case "BE":
                    be = Character.Behinderung;
                    beText = string.Format(" [BE={0}]", be);
                    break;

                case "BEx2":
                    be = Character.Behinderung * 2;
                    beText = string.Format(" [BEx2={0}]", be);
                    break;

                case "BE-2":
                    be = Character.Behinderung - 2;
                    be = be < 0 ? 0 : be;
                    beText = string.Format(" [BE-2={0}]", be);
                    break;

                case "BE-3":
                    be = Character.Behinderung - 3;
                    be = be < 0 ? 0 : be;
                    beText = string.Format(" [BE-3={0}]", be);
                    break;

                case "BE-4":
                    be = Character.Behinderung - 4;
                    be = be < 0 ? 0 : be;
                    beText = string.Format(" [BE-4={0}]", be);
                    break;

                case "0-&gt;BE":
                    be = 0;
                    beText = " [0->BE]";
                    break;

                default:
                    // special talent cases
                    switch (trialName)
                    {
                        case "Sich verkleiden":
                            beText = " [0->BEx2 manuell beachten]";
                            break;

                        case "Überreden":
                            beText = " [BE-4->BEx2 manuell beachten]";
                            break;

                        case "Überzeuen":
                            beText = " [0->BE-4 manuell beachten]";
                            break;

                        case "Orientierung":
                            beText = " [0->3BE manuell beachten]";
                            break;
                    }
                    be = 0;
                    break;
            }

            trialPointsRemaining -= be;

            #endregion Behinderung / BE

            if (trialPointsRemaining < 0)
            {
                attribute1Value += trialPointsRemaining;
                attribute2Value += trialPointsRemaining;
                attribute3Value += trialPointsRemaining;
                trialPointsRemaining = 0;
            }

            if (roll1 > attribute1Value)
            {
                trialPointsRemaining += attribute1Value - roll1;
            }

            if (roll2 > attribute2Value)
            {
                trialPointsRemaining += attribute2Value - roll2;
            }

            if (roll3 > attribute3Value)
            {
                trialPointsRemaining += attribute3Value - roll3;
            }

            #region doppel 1/20

            if (roll1 == 1)
            {
                rolledOnes++;
            }
            else if (roll1 == 20)
            {
                rolledTwenties++;
            }

            if (roll2 == 1)
            {
                rolledOnes++;
            }
            else if (roll2 == 20)
            {
                rolledTwenties++;
            }

            if (roll3 == 1)
            {
                rolledOnes++;
            }
            else if (roll3 == 20)
            {
                rolledTwenties++;
            }

            resultText = (trialPointsRemaining >= 0 ? "geschafft :)" : "nicht geschafft :(");

            if (rolledOnes == 2)
            {
                resultText = "geschafft; Doppel 1! :D";
            }
            else if (rolledOnes == 3)
            {
                resultText = "geschafft; Bei den Zwölfen, dreifach 1! :D";
            }

            if (rolledTwenties == 2)
            {
                resultText = "oh oh... doppel 20; nicht geschafft :(";
            }
            else if (rolledTwenties == 3)
            {
                resultText = "fuck... dreifach 20; War nett dich gekannt zu haben! :(";
            }

            #endregion doppel 1/20

            #region max. Erschwernis

            string erschwernisTxt = "";

            if (trialPointsRemaining >= trialValue)
            {
                int[] erschwernisTmp = { attribute1Value - roll1, attribute2Value - roll2, attribute3Value - roll3 };
                trialModificationMax = trialValue + Math.Abs(erschwernisTmp.Min());

                if (trialModificationMax > 0)
                {
                    erschwernisTxt = string.Format("(max. Erschwernis +{0})", trialModificationMax);
                }
            }

            #endregion max. Erschwernis

            if (trialPointsRemaining > trialValue)
            {
                trialPointsRemaining = trialValue; // can't have more left than what you started with (trial modifications)
            }

            if (Character.RollModeOpen == false)
                blindText = "(blind) ";

            return string.Format($"{blindText}auf {trialName}: {roll1}, {roll2}, {roll3} ⇒ {trialPointsRemaining} {resultText} {erschwernisTxt}{trialModificationTxt}{beText}");
        }

        public string RollDice(string parm, int diceAmount)
        {
            RollResult rollResult;
            string[] bedeutungen = new string[13];
            string result = string.Empty;
            string rollValueString = string.Empty;

            switch (parm)
            {
                case "d20":
                case "d12":
                case "d6":
                case "d3":
                    rollResult = Roller.Roll($"{diceAmount}{parm}");

                    result += $"{diceAmount}{parm}:";

                    foreach (DieResult singleRoll in rollResult.Values)
                    {
                        if (singleRoll.Value != 0)
                        {
                            result += $" {singleRoll.Value} +";
                        }
                    }

                    result = result.Substring(0, result.Length - 2);

                    result += $" = {rollResult.Value}";
                    break;

                case "smileydice":
                    string[] smileys = new string[] { ":)", ":|", ";)", ":(", ":'(", "]:>" };
                    rollResult = Roller.Roll("1d" + smileys.Length);

                    result = $"einen schicksalshaften Würfelwurf. Und die Würfel würfeln: {smileys[(int)rollResult.Value - 1]}";
                    break;

                case "hitzonedice":
                    string[] trefferzonen = new string[] {
                        "Kopf",
                        "Kopf",
                        "Brust",
                        "Brust",
                        "Brust",
                        "Bauch",
                        "Bauch",
                        "Rücken",
                        "Rücken",
                        "Linkes Bein",
                        "Linkes Bein",
                        "Linkes Bein",
                        "Rechtes Bein",
                        "Rechtes Bein",
                        "Rechtes Bein",
                        "Linker Arm",
                        "Linker Arm",
                        "Linker Arm",
                        "Rechter Arm",
                        "Rechter Arm",
                        "Rechter Arm",
                         };
                    rollResult = Roller.Roll("1d" + trefferzonen.Length);

                    result = $"einen Trefferzonenwürfel. Die getroffene Trefferzone ist: {trefferzonen[(int)rollResult.Value - 1]}";
                    break;

                case "meleepatzerdice":
                    rollResult = Roller.Roll("2d6");

                    foreach (DieResult singleRoll in rollResult.Values)
                    {
                        if (singleRoll.Value != 0)
                        {
                            rollValueString += $" {singleRoll.Value} +";
                        }
                    }

                    rollValueString = rollValueString.Substring(0, rollValueString.Length - 2);

                    bedeutungen[0] = string.Empty;
                    bedeutungen[1] = string.Empty;
                    bedeutungen[2] = "[2] Waffe zerstört";
                    bedeutungen[3] = "[3-5] Sturz";
                    bedeutungen[4] = bedeutungen[3];
                    bedeutungen[5] = bedeutungen[3];
                    bedeutungen[6] = "[6-8] Stolpern";
                    bedeutungen[7] = bedeutungen[6];
                    bedeutungen[8] = bedeutungen[6];
                    bedeutungen[9] = "[9-10] Waffe verloren";
                    bedeutungen[10] = bedeutungen[9];
                    bedeutungen[11] = "[11] An eigener Waffeverletzt";
                    bedeutungen[12] = "[12] Schwerer Eigentreffer";

                    result = $"auf die Nahkampf-Patzertabelle: {rollValueString} = {rollResult.Value} (Bedeutung: {bedeutungen[(int)rollResult.Value]})";
                    break;

                case "rangedpatzerdice":
                    rollResult = Roller.Roll("2d6");

                    foreach (DieResult singleRoll in rollResult.Values)
                    {
                        if (singleRoll.Value != 0)
                        {
                            rollValueString += $" {singleRoll.Value} +";
                        }
                    }

                    rollValueString = rollValueString.Substring(0, rollValueString.Length - 2);

                    bedeutungen[0] = string.Empty;
                    bedeutungen[1] = string.Empty;
                    bedeutungen[2] = "[2] Waffe zerstört";
                    bedeutungen[3] = "[3] Waffe beschädigt";
                    bedeutungen[4] = "[4-10] Fehlschuss";
                    bedeutungen[5] = bedeutungen[4];
                    bedeutungen[6] = bedeutungen[4];
                    bedeutungen[7] = bedeutungen[4];
                    bedeutungen[8] = bedeutungen[4];
                    bedeutungen[9] = bedeutungen[4];
                    bedeutungen[10] = bedeutungen[4];
                    bedeutungen[11] = "[11-12] Kameraden getroffen";
                    bedeutungen[12] = bedeutungen[11];

                    result = $"auf die Fernkampf-Patzertabelle: {rollValueString} = {rollResult.Value} (Bedeutung: {bedeutungen[(int)rollResult.Value]})";
                    break;

                case "attributMU":
                    result = RollAttributeDice(Character.MU, "Mut");
                    break;

                case "attributKL":
                    result = RollAttributeDice(Character.KL, "Klugheit");
                    break;

                case "attributIN":
                    result = RollAttributeDice(Character.IN, "Intuition");
                    break;

                case "attributCH":
                    result = RollAttributeDice(Character.CH, "Charisma");
                    break;

                case "attributFF":
                    result = RollAttributeDice(Character.FF, "Fingerfertigkeit");
                    break;

                case "attributGE":
                    result = RollAttributeDice(Character.GE, "Gewandheit");
                    break;

                case "attributKO":
                    result = RollAttributeDice(Character.KO, "Konstitution");
                    break;

                case "attributKK":
                    result = RollAttributeDice(Character.KK, "Körperkraft");
                    break;
            }

            if (Character.RollModeOpen == false)
                result = "(blind) " + result;

            return result;
        }

        private string RollAttributeDice(int attributWert, string attributTxt)
        {
            RollResult rollResult;
            int rollValue;
            string suffix;
            string ret;

            rollResult = Roller.Roll("1d20");
            rollValue = decimal.ToInt32(rollResult.Value);
            rollValue += Character.Modifikation;

            if (rollValue < 1)
                rollValue = 1;

            if (rollValue <= attributWert)
            {
                suffix = string.Format("{0} Punkte über, geschafft! :)", attributWert - rollValue);
            }
            else
            {
                suffix = string.Format("{0} Punkte drüber, nicht geschafft... :(", (attributWert - rollValue) * -1);
            }

            if (Character.Modifikation != 0)
            {
                ret = $"auf {attributTxt}: eine {rollValue} (gewürfelt {rollResult.Value} mod. {Character.Modifikation}); {suffix}";
            }
            else
            {
                ret = $"auf {attributTxt}: eine {rollValue}; {suffix}";
            }

            Character.Modifikation = 0;

            return ret;
        }

        /// <summary>
        /// Get current characters attribute value by AttributeType
        /// </summary>
        /// <param name="attribut"></param>
        /// <returns></returns>
        public int GetAttributeValue(AttributType attribut)
        {
            int attributeValue;

            switch (attribut)
            {
                case AttributType.Mut:
                    attributeValue = Character.MU;
                    break;

                case AttributType.Klugheit:
                    attributeValue = Character.KL;
                    break;

                case AttributType.Intuition:
                    attributeValue = Character.IN;
                    break;

                case AttributType.Charisma:
                    attributeValue = Character.CH;
                    break;

                case AttributType.Fingerfertigkeit:
                    attributeValue = Character.FF;
                    break;

                case AttributType.Gewandtheit:
                    attributeValue = Character.GE;
                    break;

                case AttributType.Konstitution:
                    attributeValue = Character.KO;
                    break;

                case AttributType.Koerperkraft:
                    attributeValue = Character.KK;
                    break;

                case AttributType.Wildcard:
                    attributeValue = 0;
                    break;

                default:
                    attributeValue = 0;
                    break;
            }

            return attributeValue;
        }

        /// <summary>
        /// Get the short name for a given attribute
        /// </summary>
        /// <param name="attribut"></param>
        /// <returns></returns>
        public string GetAttributeNameShort(AttributType attribut)
        {
            string attributeName;

            switch (attribut)
            {
                case AttributType.Mut:
                    attributeName = "MU";
                    break;

                case AttributType.Klugheit:
                    attributeName = "KL";
                    break;

                case AttributType.Intuition:
                    attributeName = "IN";
                    break;

                case AttributType.Charisma:
                    attributeName = "CH";
                    break;

                case AttributType.Fingerfertigkeit:
                    attributeName = "FF";
                    break;

                case AttributType.Gewandtheit:
                    attributeName = "GE";
                    break;

                case AttributType.Konstitution:
                    attributeName = "KO";
                    break;

                case AttributType.Koerperkraft:
                    attributeName = "KK";
                    break;

                case AttributType.Wildcard:
                    attributeName = "**";
                    break;

                default:
                    attributeName = "";
                    break;
            }

            return attributeName;
        }
    }
}