using Jot.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class CharacterModel : BaseModel, ITrackingAware<CharacterModel>
    {
        #region constructors

        public CharacterModel()
        {
            Attribute = new ObservableCollection<Attribut>
            {
                new Attribut() { Type = AttributType.Charisma },
                new Attribut() { Type = AttributType.Fingerfertigkeit },
                new Attribut() { Type = AttributType.Gewandtheit },
                new Attribut() { Type = AttributType.Intuition },
                new Attribut() { Type = AttributType.Klugheit },
                new Attribut() { Type = AttributType.Koerperkraft },
                new Attribut() { Type = AttributType.Konstitution },
                new Attribut() { Type = AttributType.Mut }
            };
        }

        #endregion constructors

        #region enums

        public enum AttributType
        {
            Mut,
            Klugheit,
            Intuition,
            Charisma,
            Fingerfertigkeit,
            Gewandtheit,
            Konstitution,
            Koerperkraft,
            Wildcard
        }

        public enum TalentGruppe
        {
            Waffen,
            Koerper,
            Gesellschaft,
            Natur,
            Wissen,
            Sprachen,
            Schrift,
            Handwerk,
            Meta,
            Gabe,
            Custom
        }

        #endregion enums

        #region properties

        public ObservableCollection<Attribut> Attribute { get; set; }

        public ObservableCollection<Ausbildung> Ausbildungen { get; set; }

        public int Behinderung { get; set; }

        public int JagdwaffenTaW { get; set; }

        public int Modifikation { get; set; }

        public string FileName { get; set; }

        public string Id { get; set; }

        public DateTime LastUpdate
        {
            get
            {
                double d = double.Parse(Stand);
                TimeSpan time = TimeSpan.FromMilliseconds(d);
                DateTime date = new DateTime(1970, 1, 1).AddTicks(time.Ticks);
                return date;
            }
        }

        public string Name { get; set; }

        public ObservableCollection<Sonderfertigkeit> Sonderfertigkeiten { get; set; }

        public string Stand { get; set; }

        public ObservableCollection<Talent> Talentliste { get; set; }

        public List<Talent> KoerperTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Koerper).ToList(); }
        public List<Talent> GesellschaftTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Gesellschaft).ToList(); }
        public List<Talent> NaturTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Natur).ToList(); }
        public List<Talent> WissenTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Wissen || t.Gruppe == TalentGruppe.Sprachen || t.Gruppe == TalentGruppe.Schrift).ToList(); }
        public List<Talent> HandwerkTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Handwerk).ToList(); }
        public List<Talent> MetaTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Meta || t.Gruppe == TalentGruppe.Gabe || t.Gruppe == TalentGruppe.Custom).ToList(); }

        public ObservableCollection<Vorteil> Vorteile { get; set; }

        public ObservableCollection<Zauber> Zauberliste { get; set; }

        public ObservableCollection<Heldenausruestung> Ausruestung { get; set; }

        #region static attributes

        public int CH { get => GetAttributValueByType(AttributType.Charisma); }
        public int FF { get => GetAttributValueByType(AttributType.Fingerfertigkeit); }
        public int GE { get => GetAttributValueByType(AttributType.Gewandtheit); }
        public int IN { get => GetAttributValueByType(AttributType.Intuition); }
        public int KK { get => GetAttributValueByType(AttributType.Koerperkraft); }
        public int KL { get => GetAttributValueByType(AttributType.Klugheit); }
        public int KO { get => GetAttributValueByType(AttributType.Konstitution); }
        public int MU { get => GetAttributValueByType(AttributType.Mut); }

        private int GetAttributValueByType(AttributType attributType)
        {
            if (Attribute.Any(a => a.Type == attributType))
            {
                int baseValue = Attribute.Where(a => a.Type == attributType).First().Value;
                int modValue = Attribute.Where(a => a.Type == attributType).First().Mod;

                return baseValue + modValue;
            }
            else
            {
                return 0;
            }
        }

        #endregion static attributes

        #endregion properties

        #region methods

        public void ConfigureTracking(TrackingConfiguration<CharacterModel> configuration)
        {
            configuration
                .Id(c => "LastCharacter")
                .Properties(c => new
                {
                    c.Attribute,
                    c.Ausbildungen,
                    c.Behinderung,
                    c.JagdwaffenTaW,
                    c.Modifikation,
                    c.FileName,
                    c.Id,
                    c.Name,
                    c.Sonderfertigkeiten,
                    c.Stand,
                    c.Talentliste,
                    c.Vorteile,
                    c.Zauberliste,
                    c.Ausruestung
                })
                .PersistOn(nameof(PropertyChanged));
        }

        /// <summary>
        /// copy every property of the parameter fromCharacter to the current instance of the CharacterModel
        /// </summary>
        /// <param name="fromCharacter">instance of CharacterModel</param>
        public void CopyProperties(CharacterModel fromCharacter)
        {
            foreach (var fromCharacterProperty in GetType().GetProperties())
            {
                foreach (var characterProperty in GetType().GetProperties())
                {
                    if (fromCharacterProperty.Name == characterProperty.Name &&
                        fromCharacterProperty.PropertyType == characterProperty.PropertyType &&
                        characterProperty.SetMethod != null)
                    {
                        characterProperty.SetValue(this, fromCharacterProperty.GetValue(fromCharacter));
                        break;
                    }
                }
            }
        }

        #endregion methods

        #region mappings

        public static string MapAttributeTypeToStringShort(AttributType attributType)
        {
            switch (attributType)
            {
                case AttributType.Mut:
                    return "MU";

                case AttributType.Klugheit:
                    return "KL";

                case AttributType.Intuition:
                    return "IN";

                case AttributType.Charisma:
                    return "CH";

                case AttributType.Fingerfertigkeit:
                    return "FF";

                case AttributType.Gewandtheit:
                    return "GE";

                case AttributType.Konstitution:
                    return "KO";

                case AttributType.Koerperkraft:
                    return "KK";

                case AttributType.Wildcard:
                    return "**";

                default:
                    return "";
            }
        }

        public static AttributType MapStringToAttributType(string name)
        {
            switch (name)
            {
                case "Mut":
                case "MU":
                    return AttributType.Mut;

                case "Klugheit":
                case "KL":
                    return AttributType.Klugheit;

                case "Intuition":
                case "IN":
                    return AttributType.Intuition;

                case "Charisma":
                case "CH":
                    return AttributType.Charisma;

                case "Fingerfertigkeit":
                case "FF":
                    return AttributType.Fingerfertigkeit;

                case "Gewandtheit":
                case "GE":
                    return AttributType.Gewandtheit;

                case "Konstitution":
                case "KO":
                    return AttributType.Konstitution;

                case "Körperkraft":
                case "KK":
                    return AttributType.Koerperkraft;

                default:
                    return AttributType.Wildcard;
            }
        }

        public static Probe MapStringToProbe(string probeStr)
        {
            AttributType a1;
            AttributType a2;
            AttributType a3;

            probeStr = probeStr.Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

            a1 = MapStringToAttributType(probeStr.Split('/')[0]);
            a2 = MapStringToAttributType(probeStr.Split('/')[1]);
            a3 = MapStringToAttributType(probeStr.Split('/')[2]);

            Probe convertedProbe = new Probe()
            {
                Attribut1 = a1,
                Attribut2 = a2,
                Attribut3 = a3
            };

            return convertedProbe;
        }

        public static TalentGruppe MapStringToTalentGruppe(string talentName)
        {
            switch (talentName)
            {
                case "Armbrust":
                case "Belagerungswaffen":
                case "Blasrohr":
                case "Bogen":
                case "Diskus":
                case "Schleuder":
                case "Wurfbeile":
                case "Wurfmesser":
                case "Wurfspeere":
                case "Anderthalbhänder":
                case "Dolche":
                case "Fechtwaffen":
                case "Hiebwaffen":
                case "Infanteriewaffen":
                case "Kettenstäbe":
                case "Kettenwaffen":
                case "Lanzenreiten":
                case "Peitsche":
                case "Raufen":
                case "Ringen":
                case "Säbel":
                case "Schwerter":
                case "Speere":
                case "Stäbe":
                case "Zweihandflegel":
                case "Zweihandhiebwaffen":
                case "Zweihandschwerter/-säbel":
                    return TalentGruppe.Waffen;

                case "Akrobatik":
                case "Athletik":
                case "Fliegen":
                case "Gaukeleien":
                case "Immanspiel":
                case "Klettern":
                case "Körperbeherrschung":
                case "Reiten":
                case "Schleichen":
                case "Schwimmen":
                case "Selbstbeherrschung":
                case "Sich verstecken":
                case "Singen":
                case "Sinnenschärfe":
                case "Skifahren":
                case "Stimmen imitieren":
                case "Tanzen":
                case "Taschendiebstahl":
                case "Zechen":
                    return TalentGruppe.Koerper;

                case "Betören":
                case "Etikette":
                case "Gassenwissen":
                case "Lehren":
                case "Menschenkenntnis":
                case "Schauspielerei":
                case "Schriftlicher Ausdruck":
                case "Sich verkleiden":
                case "Überreden":
                case "Überzeugen":
                    return TalentGruppe.Gesellschaft;

                case "Fährtensuchen":
                case "Fallen stellen":
                case "Fesseln/Entfesseln":
                case "Fischen/Angeln":
                case "Orientierung":
                case "Wettervorhersage":
                case "Wildnisleben":
                    return TalentGruppe.Natur;

                case "Anatomie":
                case "Baukunst":
                case "Brett-/Kartenspiel":
                case "Geografie":
                case "Geschichtswissen":
                case "Gesteinskunde":
                case "Götter und Kulte":
                case "Heraldik":
                case "Hüttenkunde":
                case "Kriegskunst":
                case "Kryptographie":
                case "Magiekunde":
                case "Mechanik":
                case "Pflanzenkunde":
                case "Philosophie":
                case "Rechnen":
                case "Rechtskunde":
                case "Sagen und Legenden":
                case "Schätzen":
                case "Schiffbau":
                case "Sprachenkunde":
                case "Staatskunst":
                case "Sternkunde":
                case "Tierkunde":
                    return TalentGruppe.Wissen;

                case "Abrichten":
                case "Ackerbau":
                case "Alchimie":
                case "Bergbau":
                case "Bogenbau":
                case "Boote fahren":
                case "Brauer":
                case "Drucker":
                case "Fahrzeug lenken":
                case "Falschspiel":
                case "Feinmechanik":
                case "Feuersteinbearbeitung":
                case "Fleischer":
                case "Gerber/Kürschner":
                case "Glaskunst":
                case "Grobschmied":
                case "Handel":
                case "Hauswirtschaft":
                case "Heilkunde: Gift":
                case "Heilkunde: Krankheiten":
                case "Heilkunde: Seele":
                case "Heilkunde: Wunden":
                case "Holzbearbeitung":
                case "Instrumentenbauer":
                case "Kartografie":
                case "Kochen":
                case "Kristallzucht":
                case "Lederarbeiten":
                case "Malen/Zeichnen":
                case "Maurer":
                case "Metallguss":
                case "Musizieren":
                case "Schlösser knacken":
                case "Schnaps brennen":
                case "Schneidern":
                case "Seefahrt":
                case "Seiler":
                case "Steinmetz":
                case "Steinschneider/Juwelier":
                case "Stellmacher":
                case "Stoffe färben":
                case "Tätowieren":
                case "Töpfern":
                case "Viehzucht":
                case "Webkunst":
                case "Winzer":
                case "Zimmermann":
                    return TalentGruppe.Handwerk;

                case "Ansitzjagd":
                case "Hetzjagd":
                case "Häuserlauf":
                case "Kräuter Suchen":
                case "Nahrung Sammeln":
                case "Pirschjagd":
                case "Speerfischen":
                case "Tierfallen stellen":
                case "Traumreise":
                case "Wassersuchen":
                case "Wache halten":
                case "Heimlichkeit":
                    return TalentGruppe.Meta;

                case "Empathie":
                case "Gefahreninstinkt":
                case "Geräuschhexerei":
                case "Kräfteschub/Talentschub":
                case "Magiegespür":
                case "Prophezeien":
                case "Tierempathie":
                case "Zwergennase":
                case "Liturgiekenntnis":
                    return TalentGruppe.Gabe;

                default:
                    if (talentName.StartsWith("Sprachen kennen"))
                    {
                        return TalentGruppe.Sprachen;
                    }
                    else if (talentName.StartsWith("Lesen/Schreiben"))
                    {
                        return TalentGruppe.Schrift;
                    }
                    else
                    {
                        return TalentGruppe.Custom;
                    }
            }
        }

        #endregion mappings

        #region structs

        public class Attribut : BaseModel
        {
            public string Name { get; set; } = "";
            public AttributType Type { get; set; } = AttributType.Wildcard;
            public int Value { get; set; } = 0;
            public int Mod { get; set; } = 0;
        }

        public class Ausbildung : BaseModel
        {
            public string Art { get; set; }
            public string Name { get; set; }
            public string Tarnidentitaet { get; set; }
        }

        public class Auswahl : BaseModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class Probe : BaseModel
        {
            public AttributType Attribut1 { get; set; }
            public AttributType Attribut2 { get; set; }
            public AttributType Attribut3 { get; set; }
        }

        public class Sonderfertigkeit : BaseModel
        {
            public string Name { get; set; }
        }

        public class Talent : BaseModel
        {
            public string Behinderungsfaktor { get; set; }
            public TalentGruppe Gruppe { get; set; }
            public string Name { get; set; }
            public Probe Probe { get; set; }
            public int Value { get; set; }
            public float Erfolgswahrscheinlichkeit { get; set; }
            public string ToolTip => $"{Name}: {MapAttributeTypeToStringShort(Probe.Attribut1)}/{MapAttributeTypeToStringShort(Probe.Attribut2)}/{MapAttributeTypeToStringShort(Probe.Attribut3)}";
        }

        public class Zauber : BaseModel
        {
            public string Name { get; set; }
            public Probe Probe { get; set; }
            public int Value { get; set; }
            public float Erfolgswahrscheinlichkeit { get; set; }
            public string ToolTip => $"{Name}: {MapAttributeTypeToStringShort(Probe.Attribut1)}/{MapAttributeTypeToStringShort(Probe.Attribut2)}/{MapAttributeTypeToStringShort(Probe.Attribut3)}";
        }

        public class Vorteil : BaseModel
        {
            public Auswahl[] Auswahl { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Spezialisierung { get => BuildSpezialisierung(); }

            private string BuildSpezialisierung()
            {
                if (Auswahl.Length > 0)
                {
                    if (string.IsNullOrEmpty(Value))
                    {
                        Value = Auswahl[0].Value;
                    }

                    return $"{Auswahl[1]?.Value}";
                }
                else
                {
                    return "";
                }
            }
        }

        public class Heldenausruestung : BaseModel
        {
            public string Name { get; set; }
            public string Set { get; set; }
            public string Slot { get; set; }
            public string Nummer { get; set; }
            public string Talent { get; set; }
            public string Waffenname { get; set; }
        }

        #endregion structs
    }
}