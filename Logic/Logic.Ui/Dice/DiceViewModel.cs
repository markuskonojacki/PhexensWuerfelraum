using System;
using Dice;
using static PhexensWuerfelraum.Logic.Ui.CharacterModel;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class DiceViewModel
    {
        public string RollTrial(Talent talent)
        {
            return RollTrial2(talent.Name, talent.Probe, talent.Value, talent.Behinderungsfaktor);
        }

        public string RollTrial(Zauber zauber)
        {
            return RollTrial2(zauber.Name, zauber.Probe, zauber.Value);
        }

        public string RollTrial2(string name, Probe trial, int punkte, string behinderungsfaktor = "")
        {
            //string resultString = "";
            //RollResult rollResult = Roller.Roll("3d20");
            //int attributEins, attributZwei, attributDrei;
            //int rollEins = 0, rollZwei = 0, rollDrei = 0;
            //int tawUeber = punkte;
            //int einser = 0, zwanziger = 0;
            //int erschwernisMax = 0;
            //string erschwernisText = "";
            //string beText = "";
            //string ergebnis = "";
            //int i = 1;

            //foreach (var roll in rollResult.Values)
            //{
            //    if (roll.DieType == DieType.Normal)
            //    {
            //        if (i == 1)
            //        {
            //            rollEins = Convert.ToInt32(roll.Value);
            //        }
            //        else if (i == 2)
            //        {
            //            rollZwei = Convert.ToInt32(roll.Value);
            //        }
            //        else if (i == 3)
            //        {
            //            rollDrei = Convert.ToInt32(roll.Value);
            //        }
            //        i++;
            //    }
            //}

            //attributEins = GetAttributeValue(trial.AttributEins);
            //attributZwei = GetAttributeValue(trial.AttributZwei);
            //attributDrei = GetAttributeValue(trial.AttributDrei);

            //int erschwernis = 0; // ToDo Erschwernis
            //if (erschwernis != 0)
            //{
            //    tawUeber -= erschwernis;
            //    erschwernisText = string.Format(" [Erschwernis von {0}]", erschwernis);
            //}

            //#region Behinderung / BE

            //int be;
            //switch (name)
            //{
            //    case "Athletik":
            //    case "Akrobatik":
            //    case "Gaukeleien":
            //    case "Klettern":
            //    case "Körperbeherrschung":
            //    case "Schwimmen":
            //    case "Tanzen":
            //    case "Taschendiebstahl":
            //        be = character.BE * 2;
            //        beText = string.Format(" [BEx2 = {0}]", be);
            //        break;

            //    case "Betören":
            //    case "Etikette":
            //    case "Reiten":
            //    case "Sich verstecken":
            //    case "Skifahren":
            //        be = character.BE - 2;
            //        be = be < 0 ? 0 : be;
            //        beText = string.Format(" [BE-2 = {0}]", be);
            //        break;

            //    case "Singen":
            //        be = character.BE - 3;
            //        be = be < 0 ? 0 : be;
            //        beText = string.Format(" [BE-3 = {0}]", be);
            //        break;

            //    case "Gassenwissen":
            //    case "Stimmen immitieren":
            //        be = character.BE - 4;
            //        be = be < 0 ? 0 : be;
            //        beText = string.Format(" [BE-4 = {0}]", be);
            //        break;

            //    case "Schleichen":
            //    case "Fliegen":
            //        be = character.BE;
            //        beText = string.Format(" [BE = {0}]", be);
            //        break;

            //    case "Fesseln/Entfesseln":
            //    case "Fährtensuchen":
            //    case "Fischen/Angeln":
            //    case "Schauspielerei":
            //    case "Sinnenschärfe":
            //    case "Sinnenschärfe FF":
            //    case "Sinnenschärfe KL":
            //    case "Wildnisleben":
            //        be = 0;
            //        beText = string.Format(" [0->BE manuell beachten]");
            //        break;

            //    case "Sich verkleiden":
            //        be = 0;
            //        beText = string.Format(" [0->BEx2 manuell beachten]");
            //        break;

            //    case "Überreden":
            //        be = 0;
            //        beText = string.Format(" [BE-4->BEx2 manuell beachten]");
            //        break;

            //    case "Überzeuen":
            //        be = 0;
            //        beText = string.Format(" [0->BE-4 manuell beachten]");
            //        break;

            //    case "Orientierung":
            //        be = 0;
            //        beText = string.Format(" [0->3BE manuell beachten]");
            //        break;

            //    default:
            //        be = 0;
            //        break;
            //}

            //tawUeber -= be;

            //#endregion Behinderung / BE

            //if (tawUeber < 0)
            //{
            //    attributEins += tawUeber;
            //    attributZwei += tawUeber;
            //    attributDrei += tawUeber;
            //    tawUeber = 0;
            //}

            //if (rollEins > attributEins)
            //{
            //    tawUeber += attributEins - rollEins;
            //}

            //if (rollZwei > attributZwei)
            //{
            //    tawUeber += attributZwei - rollZwei;
            //}

            //if (rollDrei > attributDrei)
            //{
            //    tawUeber += attributDrei - rollDrei;
            //}

            //#region doppel1/20

            //if (rollEins == 1)
            //{
            //    einser++;
            //}
            //else if (rollEins == 20)
            //{
            //    zwanziger++;
            //}

            //if (rollZwei == 1)
            //{
            //    einser++;
            //}
            //else if (rollZwei == 20)
            //{
            //    zwanziger++;
            //}

            //if (rollDrei == 1)
            //{
            //    einser++;
            //}
            //else if (rollDrei == 20)
            //{
            //    zwanziger++;
            //}

            //ergebnis = (tawUeber >= 0 ? "geschafft :)" : "nicht geschafft :(");

            //if (einser == 2)
            //{
            //    ergebnis = "geschafft; Doppel 1! :D";
            //}
            //else if (einser == 3)
            //{
            //    ergebnis = "geschafft; Holy Shit dreifach 1! O_O";
            //}

            //if (zwanziger == 2)
            //{
            //    ergebnis = "oh oh... doppel 20; nicht geschafft :(";
            //}
            //else if (zwanziger == 3)
            //{
            //    ergebnis = "fuck... dreifach 20; War nett dich gekannt zu haben! >_<";
            //}

            //#endregion doppel1/20

            //#region max. Erschwernis

            //string erschwernisTxt = "";

            //if (tawUeber >= punkte)
            //{
            //    int[] erschwernisTmp = { attributEins - rollEins, attributZwei - rollZwei, attributDrei - rollDrei };
            //    erschwernisMax = punkte + Math.Abs(erschwernisTmp.Min());

            //    if (erschwernisMax > 0)
            //    {
            //        erschwernisTxt = string.Format("(max. Erschwernis +{0})", erschwernisMax);
            //    }
            //}

            //#endregion max. Erschwernis

            //resultString = String.Format("würfelt auf {0} ({1}={2}/{3}={4}/{5}={6}): {7}, {8}, {9} => {10} {11} {12}{13}{14}",
            //    name,
            //    GetAttributeNameShort(trial.AttributEins), attributEins,
            //    GetAttributeNameShort(trial.AttributZwei), attributZwei,
            //    GetAttributeNameShort(trial.AttributDrei), attributDrei,
            //    rollEins,
            //    rollZwei,
            //    rollDrei,
            //    tawUeber,
            //    ergebnis,
            //    erschwernisTxt,
            //    erschwernisText,
            //    beText);

            //return resultString;

            return "";
        }
    }
}