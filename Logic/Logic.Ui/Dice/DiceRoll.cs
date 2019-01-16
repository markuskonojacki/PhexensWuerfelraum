using Dice;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Linq;
using static PhexensWuerfelraum.Logic.Ui.CharacterModel;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class DiceRoll
    {
        private CharacterViewModel CharacterViewModel { get; set; } = SimpleIoc.Default.GetInstance<CharacterViewModel>();
        private ChatnRollViewModel ChatnRollViewModel { get; set; } = SimpleIoc.Default.GetInstance<ChatnRollViewModel>();
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
            string resultString = "";
            RollResult rollResult = Roller.Roll("3d20");
            int attribute1Value, attribute2Value, attribute3Value;
            int roll1 = 0, roll2 = 0, roll3 = 0;
            int trialPointsRemaining = trialValue;
            int rolledOnes = 0, rolledTwenties = 0;
            int trialModificationMax = 0;
            string trialModificationTxt = "";
            string beText = "";
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
                resultText = "geschafft; Holy Shit dreifach 1! O_O";
            }

            if (rolledTwenties == 2)
            {
                resultText = "oh oh... doppel 20; nicht geschafft :(";
            }
            else if (rolledTwenties == 3)
            {
                resultText = "fuck... dreifach 20; War nett dich gekannt zu haben! >_<";
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

            resultString = String.Format($"{trialName}: {roll1}, {roll2}, {roll3} ⇒ {trialPointsRemaining} {resultText} {erschwernisTxt}{trialModificationTxt}{beText}");

            return resultString;
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

                    throw new NotImplementedException();

                //if (onCreate == false)
                //{
                //    Windows.AttributPicker picker = new Windows.AttributPicker();
                //    if (picker.ShowDialog() == true)
                //    {
                //        attributeValue = GetAttributeValue(GetAttributeByName(picker.Answer), onCreate);
                //    }
                //}
                //break;
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