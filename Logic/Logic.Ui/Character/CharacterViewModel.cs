using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using Jot;
using Jot.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml.Linq;
using static PhexensWuerfelraum.Logic.Ui.CharacterModel;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class CharacterViewModel : BaseViewModel
    {
        #region properties

        public StateTracker Tracker;
        public ListCollectionView AttributesView { get; set; }
        public CharacterModel Character { get; set; } = new CharacterModel();
        public ObservableCollection<CharacterModel> CharacterList { get; set; }
        private SettingsModel Settings { get; set; } = SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting;

        public bool IsChildWindowOpenOrNotProperty { get; set; }

        /// <summary>
        /// gets called when a character is selected in the HeroPickDialog, called via binding
        /// </summary>
        public CharacterModel SelectedCharacter
        {
            set
            {
                if (value != null)
                {
                    Character.CopyProperties(value);

                    InitAttributesView();
                    CalcSuccessChances();
                    FindJagdwaffenTaW();
                    AddMetaTalents();
                }
            }
        }

        /// <summary>
        /// Finds the designated hunting weapon and saves the talent value for the hunting meta talents
        /// </summary>
        private void FindJagdwaffenTaW()
        {
            if (Character.Ausruestung.Any(a => a.Name == "jagtwaffe" && a.Set == "0" && a.Nummer != "0"))
            {
                Heldenausruestung h1 = Character.Ausruestung.First(a => a.Name == "jagtwaffe" && a.Set == "0" && a.Nummer != "0");
                Heldenausruestung h2 = new Heldenausruestung();

                switch (h1.Nummer)
                {
                    case "1":
                        h2 = Character.Ausruestung.First(a => a.Name == "fkwaffe1" && a.Set == "0" && a.Slot == "0");
                        break;

                    case "2":
                        h2 = Character.Ausruestung.First(a => a.Name == "fkwaffe2" && a.Set == "0" && a.Slot == "0");
                        break;

                    case "3":
                        h2 = Character.Ausruestung.First(a => a.Name == "fkwaffe3" && a.Set == "0" && a.Slot == "0");
                        break;
                }
                
                if (Character.Talentliste.Any(t => t.Name == h2.Talent))
                {
                    Character.JagdwaffenTaW = Character.Talentliste.Single(t => t.Name == h2.Talent).Value;
                }
            }
            else
            {
                Character.JagdwaffenTaW = 0;
            }
        }

        /// <summary>
        /// Adds meta talents to the talents list
        /// </summary>
        private void AddMetaTalents()
        {
            // Ansitzjagd
            if (Character.Talentliste.Any(t => t.Name == "Wildnisleben") && 
                Character.Talentliste.Any(t => t.Name == "Tierkunde") && 
                Character.Talentliste.Any(t => t.Name == "Fährtensuchen") &&
                Character.Talentliste.Any(t => t.Name == "Sich verstecken"))
            {
                var faehrtensuchen = Character.Talentliste.Single(t => t.Name == "Fährtensuchen").Value;
                var sichverstecken = Character.Talentliste.Single(t => t.Name == "Sich verstecken").Value;
                var tierkunde = Character.Talentliste.Single(t => t.Name == "Tierkunde").Value;
                var wildnisleben = Character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                int talentValue = (int)Math.Round(((double)(wildnisleben + tierkunde + faehrtensuchen + sichverstecken + Character.JagdwaffenTaW) / 5), 0, MidpointRounding.AwayFromZero);
                var maxValue = new int[] { wildnisleben * 2, tierkunde * 2, faehrtensuchen * 2, sichverstecken * 2 }.Min();

                Character.Talentliste.Add(new Talent
                {
                    Name = "Ansitzjagd",
                    Probe = new Probe
                    {
                        Attribut1 = AttributType.Mut,
                        Attribut2 = AttributType.Intuition,
                        Attribut3 = AttributType.Gewandtheit
                    },
                    Value = new int[] { talentValue, maxValue }.Min(),
                    Gruppe = TalentGruppe.Meta
                });
            }

            // Pirschjagd
            if (Character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                Character.Talentliste.Any(t => t.Name == "Tierkunde") &&
                Character.Talentliste.Any(t => t.Name == "Fährtensuchen") &&
                Character.Talentliste.Any(t => t.Name == "Schleichen"))
            {
                var faehrtensuchen = Character.Talentliste.Single(t => t.Name == "Fährtensuchen").Value;
                var schleichen = Character.Talentliste.Single(t => t.Name == "Schleichen").Value;
                var tierkunde = Character.Talentliste.Single(t => t.Name == "Tierkunde").Value;
                var wildnisleben = Character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                int talentValue = (int)Math.Round(((double)(wildnisleben + tierkunde + faehrtensuchen + schleichen + Character.JagdwaffenTaW) / 5), 0, MidpointRounding.AwayFromZero);
                var maxValue = new int[] { wildnisleben * 2, tierkunde * 2, faehrtensuchen * 2, schleichen * 2 }.Min();

                Character.Talentliste.Add(new Talent
                {
                    Name = "Pirschjagd",
                    Probe = new Probe
                    {
                        Attribut1 = AttributType.Mut,
                        Attribut2 = AttributType.Intuition,
                        Attribut3 = AttributType.Gewandtheit
                    },
                    Value = new int[] { talentValue, maxValue }.Min(),
                    Gruppe = TalentGruppe.Meta
                });
            }

            // Hetzjagd
            if (Character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                Character.Talentliste.Any(t => t.Name == "Tierkunde") &&
                Character.Talentliste.Any(t => t.Name == "Reiten"))
            {
                var reiten = Character.Talentliste.Single(t => t.Name == "Reiten").Value;
                var tierkunde = Character.Talentliste.Single(t => t.Name == "Tierkunde").Value;
                var wildnisleben = Character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                int talentValue = (int)Math.Round(((double)(wildnisleben + tierkunde + (2 * reiten) + Character.JagdwaffenTaW) / 5), 0, MidpointRounding.AwayFromZero);
                var maxValue = new int[] { wildnisleben * 2, tierkunde * 2, reiten * 2 }.Min();

                Character.Talentliste.Add(new Talent
                {
                    Name = "Hetzjagd",
                    Probe = new Probe
                    {
                        Attribut1 = AttributType.Mut,
                        Attribut2 = AttributType.Intuition,
                        Attribut3 = AttributType.Gewandtheit
                    },
                    Value = new int[] { talentValue, maxValue }.Min(),
                    Gruppe = TalentGruppe.Meta
                });
            }

            // Häuserlauf
            // besitze keine der Quellen: https://de.wiki-aventurica.de/wiki/H%C3%A4userlauf

            // Traumreise
            // besitze keine der Quellen: https://de.wiki-aventurica.de/wiki/Traumreise

            // Kräuter Suchen + Nahrung Sammeln
            if (Character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                Character.Talentliste.Any(t => t.Name == "Sinnenschärfe") &&
                Character.Talentliste.Any(t => t.Name == "Pflanzenkunde"))
            {
                var pflanzenkunde = Character.Talentliste.Single(t => t.Name == "Pflanzenkunde").Value;
                var sinnenschaerfe = Character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;
                var wildnisleben = Character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                int talentValue = (int)Math.Round(((double)(wildnisleben + sinnenschaerfe + pflanzenkunde) / 3), 0, MidpointRounding.AwayFromZero);
                var maxValue = new int[] { wildnisleben * 2, sinnenschaerfe * 2, pflanzenkunde * 2 }.Min();

                Character.Talentliste.Add(new Talent
                {
                    Name = "Kräuter suchen",
                    Probe = new Probe
                    {
                        Attribut1 = AttributType.Mut,
                        Attribut2 = AttributType.Intuition,
                        Attribut3 = AttributType.Fingerfertigkeit
                    },
                    Value = new int[] { talentValue, maxValue }.Min(),
                    Gruppe = TalentGruppe.Meta
                });

                Character.Talentliste.Add(new Talent
                {
                    Name = "Nahrung sammeln",
                    Probe = new Probe
                    {
                        Attribut1 = AttributType.Mut,
                        Attribut2 = AttributType.Intuition,
                        Attribut3 = AttributType.Fingerfertigkeit
                    },
                    Value = new int[] { talentValue, maxValue }.Min(),
                    Gruppe = TalentGruppe.Meta
                });
            }

            // Wassersuchen
            // Besitze keine der Quellen: https://de.wiki-aventurica.de/wiki/Wassersuchen

            // Wache halten
            if (Character.Talentliste.Any(t => t.Name == "Selbstbeherrschung") &&
                Character.Talentliste.Any(t => t.Name == "Sinnenschärfe"))
            {
                var selbstbeherrschung = Character.Talentliste.Single(t => t.Name == "Selbstbeherrschung").Value;
                var sinnenschaerfe = Character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;

                int talentValue = (int)Math.Round(((double)(selbstbeherrschung + (2 * sinnenschaerfe) + 1) / 3), 0, MidpointRounding.AwayFromZero);
                var maxValue = new int[] { selbstbeherrschung * 2, sinnenschaerfe * 2 }.Min();

                Character.Talentliste.Add(new Talent
                {
                    Name = "Wache halten",
                    Probe = new Probe
                    {
                        Attribut1 = AttributType.Mut,
                        Attribut2 = AttributType.Intuition,
                        Attribut3 = AttributType.Konstitution
                    },
                    Value = new int[] { talentValue, maxValue }.Min(),
                    Gruppe = TalentGruppe.Meta
                });
            }

            // Custom talents
            if (Settings.AdditionalTrials)
            {
                // Heimlichkeit
                if (Character.Talentliste.Any(t => t.Name == "Sinnenschärfe") &&
                    Character.Talentliste.Any(t => t.Name == "Sich verstecken") &&
                    Character.Talentliste.Any(t => t.Name == "Schleichen"))
                {
                    var schleichen = Character.Talentliste.Single(t => t.Name == "Schleichen").Value;
                    var sichverstecken = Character.Talentliste.Single(t => t.Name == "Sich verstecken").Value;
                    var sinnenschaerfe = Character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;

                    int talentValue = (int)Math.Round(((double)(sinnenschaerfe + sichverstecken + schleichen) / 3), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { sinnenschaerfe * 2, sichverstecken * 2, schleichen * 2 }.Min();

                    Character.Talentliste.Add(new Talent
                    {
                        Name = "Heimlichkeit",
                        Probe = new Probe
                        {
                            Attribut1 = AttributType.Mut,
                            Attribut2 = AttributType.Intuition,
                            Attribut3 = AttributType.Gewandtheit
                        },
                        Value = new int[] { talentValue, maxValue }.Min(),
                        Gruppe = TalentGruppe.Custom
                    });
                }

                // Traumkontrolle
                if (Character.Talentliste.Any(t => t.Name == "Orientierung") &&
                    Character.Talentliste.Any(t => t.Name == "Sinnenschärfe") &&
                    Character.Talentliste.Any(t => t.Name == "Selbstbeherrschung"))
                {
                    var orientierung = Character.Talentliste.Single(t => t.Name == "Orientierung").Value;
                    var selbstbeherrschung = Character.Talentliste.Single(t => t.Name == "Selbstbeherrschung").Value;
                    var sinnenschaerfe = Character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;

                    int talentValue = (int)Math.Round(((double)(orientierung + (sinnenschaerfe * 2) + (selbstbeherrschung * 3)) / 6), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { orientierung * 2, sinnenschaerfe * 2, selbstbeherrschung * 2 }.Min();

                    Character.Talentliste.Add(new Talent
                    {
                        Name = "Traumkontrolle",
                        Probe = new Probe
                        {
                            Attribut1 = AttributType.Mut,
                            Attribut2 = AttributType.Mut,
                            Attribut3 = AttributType.Intuition
                        },
                        Value = new int[] { talentValue, maxValue }.Min(),
                        Gruppe = TalentGruppe.Meta
                    });
                }
            }
        }

        private bool CanLoad() => true;

        #endregion properties

        #region commands

        public RelayCommand OpenHeroPickDialogCommand { get; set; }
        public RelayCommand CalculateSuccessChances { get; set; }

        #endregion commands

        #region mappings

        public AttributType MapStringToAttribut(string name)
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

        private Probe MapStringToProbe(string probeStr)
        {
            AttributType a1 = AttributType.Intuition;
            AttributType a2 = AttributType.Intuition;
            AttributType a3 = AttributType.Intuition;

            probeStr = probeStr.Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

            a1 = MapStringToAttribut(probeStr.Split('/')[0]);
            a2 = MapStringToAttribut(probeStr.Split('/')[1]);
            a3 = MapStringToAttribut(probeStr.Split('/')[2]);

            Probe convertedProbe = new Probe()
            {
                Attribut1 = a1,
                Attribut2 = a2,
                Attribut3 = a3
            };

            return convertedProbe;
        }

        private TalentGruppe MapStringToTalentGruppe(string talentName)
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

        public CharacterViewModel()
        {
            #region define current Character and load from settings file

            var charactersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum");
            Directory.CreateDirectory(charactersFilePath);

            Tracker = new StateTracker() { StoreFactory = new JsonFileStoreFactory(charactersFilePath) };
            Tracker.Configure(Character).Apply();

            #endregion define current Character and load from settings file

            OpenHeroPickDialogCommand = new RelayCommand(() => ImportCharacter());
            CalculateSuccessChances = new RelayCommand(() => CalcSuccessChances());

            InitAttributesView();

            Log.Instance.Trace("CharacterViewModel initialized");
        }

        /// <summary>
        /// calculate chance of success for every talent
        /// </summary>
        private void CalcSuccessChances()
        {
            // add chance of success to talent
            foreach (var talent in Character.Talentliste)
            {
                talent.Erfolgswahrscheinlichkeit = CalcSuccessChance(
                    Character.Attribute.First(a => a.Type == talent.Probe.Attribut1).Value,
                    Character.Attribute.First(a => a.Type == talent.Probe.Attribut2).Value,
                    Character.Attribute.First(a => a.Type == talent.Probe.Attribut3).Value,
                    talent.Value
                    );
            }
        }

        /// <summary>
        /// calculate chance of success
        /// function by http://simia.net/dsa4wk/
        /// </summary>
        /// <param name="e1">Attribut 1</param>
        /// <param name="e2">Attribut 2</param>
        /// <param name="e3">Attribut 3</param>
        /// <param name="taw">Talentwert</param>
        /// <param name="mod">Erleichterung/Erschwernis</param>
        /// <returns>chance of success</returns>
        private float CalcSuccessChance(int e1, int e2, int e3, int taw, int mod = 0)
        {
            float erfolgswahrscheinlichkeit = 0;
            float erfolge = 0;
            int x;
            int y;
            int z;

            for (x = 1; x <= 20; x++)
            {
                for (y = 1; y <= 20; y++)
                {
                    for (z = 1; z <= 20; z++)
                    {
                        var erfolg = false;

                        if ((x == 1 && (y == 1 || z == 1)) || (y == 1 && z == 1)) // Meisterlich
                        {
                            erfolg = true;
                        }
                        else if ((x == 20 && (y == 20 || z == 20)) || (y == 20 && z == 20)) // Patzer
                        {
                            erfolg = false;
                        }
                        else
                        {
                            var p = taw - mod;

                            if (p < 1)
                            {
                                erfolg = (x <= e1 + p && y <= e2 + p && z <= e3 + p);
                            }
                            else
                            {
                                if (x > e1)
                                {
                                    p = p - (x - e1);
                                }

                                if (y > e2)
                                {
                                    p = p - (y - e2);
                                }

                                if (z > e3)
                                {
                                    p = p - (z - e3);
                                }

                                erfolg = (p > -1);
                            }
                        }

                        if (erfolg)
                        {
                            erfolge++;
                        }
                    }
                }
            }

            erfolgswahrscheinlichkeit = erfolge / 8000;

            return erfolgswahrscheinlichkeit * 100;
        }

        #region AttributeView methods

        /// <summary>
        /// Initializes the attribute view
        /// </summary>
        public void InitAttributesView()
        {
            AttributesView = CollectionViewSource.GetDefaultView(Character.Attribute) as ListCollectionView;
            AttributesView.CurrentChanged += (s, e) =>
            {
                RaisePropertyChanged(() => Character);
            };
            AttributesView.Filter = AttributesFilter;
        }

        /// <summary>
        /// filter irrelevant attributes from view
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if it will be shown, false otherwise</returns>
        private bool AttributesFilter(object item)
        {
            Attribut attribute = item as Attribut;

            if (attribute.Type == AttributType.Wildcard)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion AttributeView methods

        /// <summary>
        /// import character from helden.zip.hld file
        /// </summary>
        public void ImportCharacter()
        {
            Log.Instance.Trace("Character import started");

            CharacterList = new ObservableCollection<CharacterModel>();
            string filename = SimpleIoc.Default.GetInstance<SettingsViewModel>().Setting.HeldenDateiPath;

            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                MessengerInstance.Send(new OpenInfoMessage("Charakter Import", "Bitte wähle zuerst einen gültigen Pfad zur helden.zip.hld im Einstellungsmenü"));
            }

            if (File.Exists(filename))
            {
                using (ZipArchive zip = ZipFile.Open(filename, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.Name != "held.xml.tree")
                        {
                            StreamReader characterStreamReader = new StreamReader(entry.Open(), Encoding.UTF8);
                            XDocument doc = XDocument.Parse(characterStreamReader.ReadToEnd());

                            var pickCharacter = XmlToCharacterModel(doc);

                            pickCharacter.FileName = entry.FullName;

                            CharacterList.Add(pickCharacter);
                        }
                    }

                    CharacterList = new ObservableCollection<CharacterModel>(CharacterList.OrderByDescending(c => c.Stand));

                    MessengerInstance.Send(new OpenHeroPickDialogMessage());
                }
            }
        }

        /// <summary>
        /// convert a given character xml document to a CharacterModel object
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private CharacterModel XmlToCharacterModel(XDocument doc)
        {
            CharacterModel character =
                (
                    from e in doc.Root.Elements("held")
                    select new CharacterModel
                    {
                        Name = (string)e.Attribute("name"),
                        Stand = (string)e.Attribute("stand"),
                        Id = (string)e.Attribute("key"),
                        Ausbildungen = new ObservableCollection<Ausbildung>(
                        (
                            from ausbildung in e.Elements("basis").Elements("ausbildungen").Elements("ausbildung")
                            select new Ausbildung
                            {
                                Art = (string)ausbildung.Attribute("art"),
                                Name = (string)ausbildung.Attribute("string"),
                                Tarnidentitaet = (string)ausbildung.Attribute("tarnidentitaet")
                            }).ToList()),
                        Attribute = new ObservableCollection<Attribut>(
                        (
                            from attribut in e.Elements("eigenschaften").Elements("eigenschaft")
                            select new Attribut
                            {
                                Name = (string)attribut.Attribute("name"),
                                Value = (int)attribut.Attribute("value"),
                                Type = MapStringToAttribut((string)attribut.Attribute("name"))
                            }).ToList()),
                        Vorteile = new ObservableCollection<Vorteil>(
                        (
                            from vorteil in e.Elements("vt").Elements("vorteil")
                            select new Vorteil
                            {
                                Name = (string)vorteil.Attribute("name"),
                                Value = (string)vorteil.Attribute("value"),
                                Auswahl =
                                (
                                    from auswahl in vorteil.Elements("auswahl")
                                    select new Auswahl
                                    {
                                        Name = (string)auswahl.Attribute("name"),
                                        Value = (string)auswahl.Attribute("value")
                                    }).ToArray(),
                            }).ToList()),
                        Sonderfertigkeiten = new ObservableCollection<Sonderfertigkeit>(
                        (
                            from sonderfertigkeit in e.Elements("sf").Elements("sonderfertigkeit")
                            select new Sonderfertigkeit
                            {
                                Name = (string)sonderfertigkeit.Attribute("name")
                            }).ToList()),
                        Talentliste = new ObservableCollection<Talent>(
                        (
                            from talent in e.Elements("talentliste").Elements("talent")
                            select new Talent
                            {
                                Name = (string)talent.Attribute("name"),
                                Value = (int)talent.Attribute("value"),
                                Probe = MapStringToProbe((string)talent.Attribute("probe")),
                                Behinderungsfaktor = (string)talent.Attribute("be"),
                                Gruppe = MapStringToTalentGruppe((string)talent.Attribute("name"))
                            }).ToList()),
                        Zauberliste = new ObservableCollection<Zauber>(
                        (
                            from zauber in e.Elements("zauberliste").Elements("zauber")
                            select new Zauber
                            {
                                Name = (string)zauber.Attribute("name"),
                                Value = (int)zauber.Attribute("value"),
                                Probe = MapStringToProbe((string)zauber.Attribute("probe"))
                            }).ToList()),
                        Ausruestung = new ObservableCollection<Heldenausruestung>(
                        (
                            from heldenausruestung in e.Elements("ausrüstungen").Elements("heldenausruestung")
                            select new Heldenausruestung
                            {
                                Name = (string)heldenausruestung.Attribute("name"),
                                Set = (string)heldenausruestung.Attribute("set"),
                                Slot = (string)heldenausruestung.Attribute("slot"),
                                Nummer = (string)heldenausruestung.Attribute("nummer"),
                                Talent = (string)heldenausruestung.Attribute("talent"),
                                Waffenname = (string)heldenausruestung.Attribute("waffenname"),
                            }).ToList()),
                    }).First();

#if DEBUG
            Directory.CreateDirectory(@"c:\temp\");
            var path = $@"c:\temp\{character.Name}.xml";
            doc.Save(path);
#endif

            return character;
        }
    }
}