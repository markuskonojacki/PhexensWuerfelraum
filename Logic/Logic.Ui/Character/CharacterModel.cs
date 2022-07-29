using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Jot.Configuration;
using PropertyChanged;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class CharacterModel : BaseModel, ITrackingAware
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

        public bool RollModeOpen { get; set; } = true;

        [DependsOn("Modifikation")]
        public bool ShowErleichterungLabel { get { return Modifikation < 0; } }

        [DependsOn("Modifikation")]
        public bool ShowErschwernisLabel { get { return Modifikation > 0; } }

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

        public CharacterModel CharacterPlayer1 { get; set; }
        public CharacterModel CharacterPlayer2 { get; set; }
        public CharacterModel CharacterPlayer3 { get; set; }
        public CharacterModel CharacterPlayer4 { get; set; }
        public CharacterModel CharacterPlayer5 { get; set; }
        public CharacterModel CharacterPlayer6 { get; set; }
        public CharacterModel CharacterPlayer7 { get; set; }

        public List<Talent> Player1Talentliste { get => CharacterPlayer1?.Talentliste?.ToList(); }
        public List<Talent> Player2Talentliste { get => CharacterPlayer2?.Talentliste?.ToList(); }
        public List<Talent> Player3Talentliste { get => CharacterPlayer3?.Talentliste?.ToList(); }
        public List<Talent> Player4Talentliste { get => CharacterPlayer4?.Talentliste?.ToList(); }
        public List<Talent> Player5Talentliste { get => CharacterPlayer5?.Talentliste?.ToList(); }
        public List<Talent> Player6Talentliste { get => CharacterPlayer6?.Talentliste?.ToList(); }
        public List<Talent> Player7Talentliste { get => CharacterPlayer7?.Talentliste?.ToList(); }
        public static bool IsMaster { get => SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting.GameMasterMode; }

        private static readonly List<string> moeglicheVorteile = new() { "Adlige Abstammung", "Adliges Erbe", "Amtsadel", "Affinität zu", "Akademische Ausbildung", "Astrale Regeneration", "Astralmacht", "Ausdauernd", "Ausdauernder Zauberer", "Ausrüstungsvorteil", "Balance", "Begabung für", "Beidhändig", "Beseelte Knochenkeule", "Besonderer Besitz", "Breitgefächerte Bildung", "Dämmerungssicht", "Eidetisches Gedächtnis", "Eigeboren", "Eisenaffine Aura", "Eisern", "Empathie", "Entfernungssinn", "Ererbte Knochenkeule", "Feenfreund", "Feste Matrix", "Flink", "Gebildet", "Gefahreninstinkt", "Geräuschhexerei", "Geweiht", "Glück", "Glück im Spiel", "Gut Aussehend", "Guter Ruf", "Gutes Gedächtnis", "Halbzauberer", "Herausragend", "Hitzeresistenz", "Hohe Lebenskraft", "Hohe Magieresistenz", "Immunität", "Innerer Kompass", "Kälteresistenz", "Kampfrausch", "Koboldfreund", "Kräfteschub", "Linkshänder", "Machtvoller Vertrauter", "Magiegespür", "Meisterhandwerk", "Nachtsicht", "Natürliche Waffen", "Natürlicher Rüstungsschutz", "Niedrige Schlechte Eigenschaft", "Prophezeien", "Resistenz", "Richtungssinn", "Schlangenmensch ", "Schnelle Heilung", "Viertelzauberer", "Vollzauberer", "Vom Schicksal begünstigt", "Wesen der Nacht", "Wohlklang", "Wolfskind", "Zäher Hund", "Zauberhaar", "Zeitgefühl", "Zusätzliche Gliedmaßen", "Zweistimmiger Gesang", "Zwergennase", "Schutzgeist", "Schwer zu verzaubern ", "Soziale Anpassungsfähigkeit", "Talentschub", "Tierempathie", "Tierfreund", "Übernatürliche Begabung", "Unbeschwertes Zaubern", "Verbindungen", "Verhüllte Aura", "Veteran", "Viertelzauberer", "Altersresistenz", "Gutaussehend" };

        public ObservableCollection<Vorteil> Vorteile { get; set; }

        public List<Vorteil> ListeVorteile { get => Vorteile?.Where(t => moeglicheVorteile.Any(v => t.Name.StartsWith(v))).ToList(); }
        public List<Vorteil> ListeNachteile { get => Vorteile?.Where(t => !moeglicheVorteile.Any(v => t.Name.StartsWith(v))).ToList(); }

        public ObservableCollection<Zauber> Zauberliste { get; set; }

        public ObservableCollection<Heldenausruestung> Ausruestung { get; set; }

        #region wounds
        public bool WoundHead1 { get; set; }
        public bool WoundHead2 { get; set; }
        public bool WoundHead3 { get; set; }

        public bool WoundChest1 { get; set; }
        public bool WoundChest2 { get; set; }
        public bool WoundChest3 { get; set; }

        public bool WoundLeftArm1 { get; set; }
        public bool WoundLeftArm2 { get; set; }
        public bool WoundLeftArm3 { get; set; }

        public bool WoundRightArm1 { get; set; }
        public bool WoundRightArm2 { get; set; }
        public bool WoundRightArm3 { get; set; }

        public bool WoundLeftLeg1 { get; set; }
        public bool WoundLeftLeg2 { get; set; }
        public bool WoundLeftLeg3 { get; set; }

        public bool WoundRightLeg1 { get; set; }
        public bool WoundRightLeg2 { get; set; }
        public bool WoundRightLeg3 { get; set; }

        public bool WoundStomach1 { get; set; }
        public bool WoundStomach2 { get; set; }
        public bool WoundStomach3 { get; set; }

        public string WoundsText { get { return BuildWoundsText(); } }

        private string BuildWoundsText()
        {
            string ret = "";

            #region head
            if (WoundHead1 || WoundHead2 || WoundHead3)
            {
                ret += "Kopf (1&2): MU, KL, IN, INI-Basis –2, INI –2W6\n";
            }

            if (WoundHead1 && WoundHead2 && WoundHead3)
            {
                ret += "Kopf (3): +2W6 SP, bewusstlos, Blutverlust\n";
            }
            #endregion

            #region chest
            if (WoundChest1 || WoundChest2 || WoundChest3)
            {
                ret += "Brust (1&2): AT, PA, KO, KK –1; +1W6 SP\n";
            }

            if (WoundChest1 && WoundChest2 && WoundChest3)
            {
                ret += "Brust (3): bewusstlos, Blutverlust\n";
            }
            #endregion

            #region stomach
            if (WoundStomach1 || WoundStomach2 || WoundStomach3)
            {
                ret += "Bauch (1&2): AT, PA, KO, KK, GS, INI-Basis –1; +1W6 SP\n";
            }

            if (WoundStomach1 && WoundStomach2 && WoundStomach3)
            {
                ret += "Bauch (3): bewusstlos, Blutverlust\n";
            }
            #endregion

            #region left legs
            if (WoundLeftLeg1 || WoundLeftLeg2 || WoundLeftLeg3)
            {
                ret += "Linkes Bein (1&2): AT, PA, GE, INI-Basis –2; GS –1\n";
            }

            if (WoundLeftLeg1 && WoundLeftLeg2 && WoundLeftLeg3)
            {
                ret += "Linkes Bein (3): Sturz, kampfunfähig\n";
            }
            #endregion

            #region right legs
            if (WoundRightLeg1 || WoundRightLeg2 || WoundRightLeg3)
            {
                ret += "Rechtes Bein (1&2): AT, PA, GE, INI-Basis –2; GS –1\n";
            }

            if (WoundRightLeg1 && WoundRightLeg2 && WoundRightLeg3)
            {
                ret += "Rechtes Bein (3): Sturz, kampfunfähig\n";
            }
            #endregion

            #region left arm
            if (WoundLeftArm1 || WoundLeftArm2 || WoundLeftArm3)
            {
                ret += "Linker Arm (1&2): AT, PA, KK, FF –2 mit diesem Arm\n";
            }

            if (WoundLeftArm1 && WoundLeftArm2 && WoundLeftArm3)
            {
                ret += "Linker Arm (3): Arm handlungsunfähig\n";
            }
            #endregion

            #region right arm
            if (WoundRightArm1 || WoundRightArm2 || WoundRightArm3)
            {
                ret += "Rechter Arm (1&2): AT, PA, KK, FF –2 mit diesem Arm\n";
            }

            if (WoundRightArm1 && WoundRightArm2 && WoundRightArm3)
            {
                ret += "Rechter Arm (3): Arm handlungsunfähig\n";
            }
            #endregion

            return ret;
        }

        #endregion

        #region static attributes

        public int CH => GetAttributValueByType(AttributType.Charisma) + Attribute.First(a => a.Type == AttributType.Charisma).TempMod;
        public int FF => GetAttributValueByType(AttributType.Fingerfertigkeit) + Attribute.First(a => a.Type == AttributType.Fingerfertigkeit).TempMod;
        public int GE => GetAttributValueByType(AttributType.Gewandtheit) + Attribute.First(a => a.Type == AttributType.Gewandtheit).TempMod;
        public int IN => GetAttributValueByType(AttributType.Intuition) + Attribute.First(a => a.Type == AttributType.Intuition).TempMod;
        public int KK => GetAttributValueByType(AttributType.Koerperkraft) + Attribute.First(a => a.Type == AttributType.Koerperkraft).TempMod;
        public int KL => GetAttributValueByType(AttributType.Klugheit) + Attribute.First(a => a.Type == AttributType.Klugheit).TempMod;
        public int KO => GetAttributValueByType(AttributType.Konstitution) + Attribute.First(a => a.Type == AttributType.Konstitution).TempMod;
        public int MU => GetAttributValueByType(AttributType.Mut) + Attribute.First(a => a.Type == AttributType.Mut).TempMod;

        private int GetAttributValueByType(AttributType attributType)
        {
            if (Attribute.Any(a => a.Type == attributType))
            {
                int baseValue = Attribute.First(a => a.Type == attributType).Value;
                int modValue = Attribute.First(a => a.Type == attributType).Mod;

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

        public void ConfigureTracking(TrackingConfiguration configuration)
        {
            configuration
                .Id(c => "LastCharacter")
                .Properties<CharacterModel>(c => new
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
                    c.Ausruestung,
                    c.WoundHead1,
                    c.WoundHead2,
                    c.WoundHead3,
                    c.WoundChest1,
                    c.WoundChest2,
                    c.WoundChest3,
                    c.WoundLeftArm1,
                    c.WoundLeftArm2,
                    c.WoundLeftArm3,
                    c.WoundRightArm1,
                    c.WoundRightArm2,
                    c.WoundRightArm3,
                    c.WoundLeftLeg1,
                    c.WoundLeftLeg2,
                    c.WoundLeftLeg3,
                    c.WoundRightLeg1,
                    c.WoundRightLeg2,
                    c.WoundRightLeg3,
                    c.WoundStomach1,
                    c.WoundStomach2,
                    c.WoundStomach3
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
            return attributType switch
            {
                AttributType.Mut => "MU",
                AttributType.Klugheit => "KL",
                AttributType.Intuition => "IN",
                AttributType.Charisma => "CH",
                AttributType.Fingerfertigkeit => "FF",
                AttributType.Gewandtheit => "GE",
                AttributType.Konstitution => "KO",
                AttributType.Koerperkraft => "KK",
                AttributType.Wildcard => "**",
                _ => "",
            };
        }

        public static string MapAttributeTypeToString(AttributType attributType)
        {
            return attributType switch
            {
                AttributType.Mut => "Mut",
                AttributType.Klugheit => "Klugheit",
                AttributType.Intuition => "Intuition",
                AttributType.Charisma => "Charisma",
                AttributType.Fingerfertigkeit => "Fingerfertigkeit",
                AttributType.Gewandtheit => "Gewandtheit",
                AttributType.Konstitution => "Konstitution",
                AttributType.Koerperkraft => "Körperkraft",
                AttributType.Wildcard => "**",
                _ => "",
            };
        }

        public static AttributType MapStringToAttributType(string name)
        {
            return name switch
            {
                "Mut" or "MU" => AttributType.Mut,
                "Klugheit" or "KL" => AttributType.Klugheit,
                "Intuition" or "IN" => AttributType.Intuition,
                "Charisma" or "CH" => AttributType.Charisma,
                "Fingerfertigkeit" or "FF" => AttributType.Fingerfertigkeit,
                "Gewandtheit" or "GE" => AttributType.Gewandtheit,
                "Konstitution" or "KO" => AttributType.Konstitution,
                "Körperkraft" or "KK" => AttributType.Koerperkraft,
                _ => AttributType.Wildcard,
            };
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

            Probe convertedProbe = new()
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
            public string NameShort { get => MapAttributeTypeToStringShort(Type); }
            public AttributType Type { get; set; } = AttributType.Wildcard;
            public int Value { get; set; } = 0;
            public int Mod { get; set; } = 0; // Permanente Modifikatoren vom Einlesen
            public int TempMod { get; set; } = 0; // Temporäre Modifikatoren, z.B. durch Wunden
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
            public string Spezialisierung => BuildSpezialisierung();
            public string Text => BuildText();

            private string BuildText()
            {
                string ret;

                ret = Name;

                if (string.IsNullOrEmpty(Value) == false)
                {
                    ret += $": {Value}";
                }

                if (string.IsNullOrEmpty(Spezialisierung) == false)
                {
                    ret += $" ({Spezialisierung})";
                }

                return ret;
            }

            private string BuildSpezialisierung()
            {
                if (Auswahl.Length > 0)
                {
                    if (string.IsNullOrEmpty(Value))
                    {
                        Value = Auswahl[0].Value;
                    }

                    if (Auswahl.Length > 1)
                    {
                        return $"{Auswahl[1]?.Value}";
                    }
                    else
                    {
                        return "";
                    }
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