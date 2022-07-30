using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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

        public Tracker Tracker;

        public ListCollectionView AttributesView { get; set; }
        public CharacterModel Character { get; set; } = new CharacterModel();

        public ObservableCollection<CharacterModel> CharacterList { get; set; } = new ObservableCollection<CharacterModel>
        {
            new CharacterModel()
            {
                Name = string.IsNullOrEmpty(Ioc.Default.GetService<SettingsViewModel>().Setting.StaticUserName) ? Environment.UserName :Ioc.Default.GetService<SettingsViewModel>().Setting.StaticUserName,
                Id = "StaticCharacter",
                Stand = int.MaxValue.ToString()
            }
        };

        private readonly SettingsViewModel SettingsViewModel;
        private readonly SettingsModel Settings;

        public bool IsChildWindowOpenOrNotProperty { get; set; }

        /// <summary>
        /// gets called when a character is selected, called via binding
        /// </summary>
        public CharacterModel SelectedCharacter
        {
            set
            {
                if (value != null)
                {
                    Character.CopyProperties(value);
                    InitAttributesView();
                }
            }
            get
            {
                CharacterModel ret;

                if (CharacterList.FirstOrDefault(c => c.Id == Character.Id) != null)
                {
                    ret = CharacterList.FirstOrDefault(c => c.Id == Character.Id);
                }
                else
                {
                    ret = CharacterList.First(c => c.Id == "StaticCharacter");
                }

                return ret;
            }
        }

        #endregion properties

        #region commands

        public RelayCommand CalculateSuccessChances { get; set; }

        #endregion commands

        public CharacterViewModel()
        {
            SettingsViewModel = Ioc.Default.GetService<SettingsViewModel>();
            Settings = Ioc.Default.GetService<SettingsViewModel>().Setting;

            #region define current Character and load from settings file

            Directory.CreateDirectory(SettingsViewModel.DataPath);

            Tracker = new Tracker(new JsonFileStore(SettingsViewModel.DataPath));
            Tracker.Configure<CharacterModel>();
            Tracker.Track(Character);

            #endregion define current Character and load from settings file

            CalculateSuccessChances = new RelayCommand(() => CalcSuccessChances(Character));

            InitAttributesView();

            LoadCharacterList();
        }

        /// <summary>
        /// calculate chance of success for every talent
        /// </summary>
        private static void CalcSuccessChances(CharacterModel character)
        {
            if (character.Talentliste != null)
            {
                foreach (var talent in character.Talentliste)
                {
                    talent.Erfolgswahrscheinlichkeit = CalcSuccessChance(
                        character.Attribute.First(a => a.Type == talent.Probe.Attribut1).Value,
                        character.Attribute.First(a => a.Type == talent.Probe.Attribut2).Value,
                        character.Attribute.First(a => a.Type == talent.Probe.Attribut3).Value,
                        talent.Value
                        );
                }
            }

            if (character.Zauberliste != null)
            {
                foreach (var zauber in character.Zauberliste)
                {
                    zauber.Erfolgswahrscheinlichkeit = CalcSuccessChance(
                        character.Attribute.First(a => a.Type == zauber.Probe.Attribut1).Value,
                        character.Attribute.First(a => a.Type == zauber.Probe.Attribut2).Value,
                        character.Attribute.First(a => a.Type == zauber.Probe.Attribut3).Value,
                        zauber.Value
                        );
                }
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
        private static float CalcSuccessChance(int e1, int e2, int e3, int taw, int mod = 0)
        {
            float erfolgswahrscheinlichkeit;
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
                        bool erfolg;
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
                                    p -= (x - e1);
                                }

                                if (y > e2)
                                {
                                    p -= (y - e2);
                                }

                                if (z > e3)
                                {
                                    p -= (z - e3);
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
                OnPropertyChanged(nameof(Character));
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
        public void LoadCharacterList()
        {
            // remove all but StaticCharacter, which is always at position 0
            while (CharacterList.Count > 1)
            {
                CharacterList.RemoveAt(1);
            }

            string filename = Settings.HeldenDateiPath;

            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                Messenger.Send(new OpenInfoMessage("Charakter Import", "Bitte wähle einen gültigen Pfad zur helden.zip.hld im Einstellungsmenü"));
            }

            if (File.Exists(filename))
            {
                using ZipArchive zip = ZipFile.Open(filename, ZipArchiveMode.Read);
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.Name != "held.xml.tree")
                    {
                        using StreamReader characterStreamReader = new(entry.Open(), Encoding.UTF8);
                        XDocument doc = XDocument.Parse(characterStreamReader.ReadToEnd());

                        var pickCharacter = XmlToCharacterModel(doc);

                        pickCharacter.FileName = entry.FullName;

                        CharacterList.Add(pickCharacter);
                    }
                }

                CharacterList = new ObservableCollection<CharacterModel>(CharacterList.OrderByDescending(c => c.Stand));
            }
        }

        /// <summary>
        /// Finds the designated hunting weapon and saves the talent value for the hunting meta talents
        /// </summary>
        private static void FindJagdwaffenTaW(CharacterModel character)
        {
            // "jagtwaffe" is the term used in the xml
            if (character.Ausruestung != null && character.Ausruestung.Any(a => a.Name == "jagtwaffe" && a.Set == "0" && a.Nummer != "0"))
            {
                Heldenausruestung h1 = character.Ausruestung.First(a => a.Name == "jagtwaffe" && a.Set == "0" && a.Nummer != "0");
                Heldenausruestung h2 = new();

                switch (h1.Nummer)
                {
                    case "1":
                        if (character.Ausruestung.Any(a => a.Name == "fkwaffe1" && a.Set == "0" && a.Slot == "0"))
                        {
                            h2 = character.Ausruestung.First(a => a.Name == "fkwaffe1" && a.Set == "0" && a.Slot == "0");
                        }
                        break;

                    case "2":
                        if (character.Ausruestung.Any(a => a.Name == "fkwaffe2" && a.Set == "0" && a.Slot == "0"))
                        {
                            h2 = character.Ausruestung.First(a => a.Name == "fkwaffe2" && a.Set == "0" && a.Slot == "0");
                        }
                        break;

                    case "3":
                        if (character.Ausruestung.Any(a => a.Name == "fkwaffe3" && a.Set == "0" && a.Slot == "0"))
                        {
                            h2 = character.Ausruestung.First(a => a.Name == "fkwaffe3" && a.Set == "0" && a.Slot == "0");
                        }
                        break;
                }

                if (character.Talentliste.Any(t => t.Name == h2.Talent))
                {
                    character.JagdwaffenTaW = character.Talentliste.Single(t => t.Name == h2.Talent).Value;
                }
            }
            else
            {
                character.JagdwaffenTaW = 0;
            }
        }

        /// <summary>
        /// Adds meta talents to the talents list
        /// </summary>
        private static void AddMetaTalents(CharacterModel character)
        {
            if (character.Talentliste != null)
            {
                // Ansitzjagd
                if (character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                    character.Talentliste.Any(t => t.Name == "Tierkunde") &&
                    character.Talentliste.Any(t => t.Name == "Fährtensuchen") &&
                    character.Talentliste.Any(t => t.Name == "Sich verstecken"))
                {
                    var faehrtensuchen = character.Talentliste.Single(t => t.Name == "Fährtensuchen").Value;
                    var sichverstecken = character.Talentliste.Single(t => t.Name == "Sich verstecken").Value;
                    var tierkunde = character.Talentliste.Single(t => t.Name == "Tierkunde").Value;
                    var wildnisleben = character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                    int talentValue = (int)Math.Round(((double)(wildnisleben + tierkunde + faehrtensuchen + sichverstecken + character.JagdwaffenTaW) / 5), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { wildnisleben * 2, tierkunde * 2, faehrtensuchen * 2, sichverstecken * 2 }.Min();

                    character.Talentliste.Add(new Talent
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
                if (character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                    character.Talentliste.Any(t => t.Name == "Tierkunde") &&
                    character.Talentliste.Any(t => t.Name == "Fährtensuchen") &&
                    character.Talentliste.Any(t => t.Name == "Schleichen"))
                {
                    var faehrtensuchen = character.Talentliste.Single(t => t.Name == "Fährtensuchen").Value;
                    var schleichen = character.Talentliste.Single(t => t.Name == "Schleichen").Value;
                    var tierkunde = character.Talentliste.Single(t => t.Name == "Tierkunde").Value;
                    var wildnisleben = character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                    int talentValue = (int)Math.Round(((double)(wildnisleben + tierkunde + faehrtensuchen + schleichen + character.JagdwaffenTaW) / 5), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { wildnisleben * 2, tierkunde * 2, faehrtensuchen * 2, schleichen * 2 }.Min();

                    character.Talentliste.Add(new Talent
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
                if (character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                    character.Talentliste.Any(t => t.Name == "Tierkunde") &&
                    character.Talentliste.Any(t => t.Name == "Reiten"))
                {
                    var reiten = character.Talentliste.Single(t => t.Name == "Reiten").Value;
                    var tierkunde = character.Talentliste.Single(t => t.Name == "Tierkunde").Value;
                    var wildnisleben = character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                    int talentValue = (int)Math.Round(((double)(wildnisleben + tierkunde + (2 * reiten) + character.JagdwaffenTaW) / 5), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { wildnisleben * 2, tierkunde * 2, reiten * 2 }.Min();

                    character.Talentliste.Add(new Talent
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
                if (character.Talentliste.Any(t => t.Name == "Wildnisleben") &&
                    character.Talentliste.Any(t => t.Name == "Sinnenschärfe") &&
                    character.Talentliste.Any(t => t.Name == "Pflanzenkunde"))
                {
                    var pflanzenkunde = character.Talentliste.Single(t => t.Name == "Pflanzenkunde").Value;
                    var sinnenschaerfe = character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;
                    var wildnisleben = character.Talentliste.Single(t => t.Name == "Wildnisleben").Value;

                    int talentValue = (int)Math.Round(((double)(wildnisleben + sinnenschaerfe + pflanzenkunde) / 3), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { wildnisleben * 2, sinnenschaerfe * 2, pflanzenkunde * 2 }.Min();

                    character.Talentliste.Add(new Talent
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

                    character.Talentliste.Add(new Talent
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
                if (character.Talentliste.Any(t => t.Name == "Selbstbeherrschung") &&
                    character.Talentliste.Any(t => t.Name == "Sinnenschärfe"))
                {
                    var selbstbeherrschung = character.Talentliste.Single(t => t.Name == "Selbstbeherrschung").Value;
                    var sinnenschaerfe = character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;

                    int talentValue = (int)Math.Round(((double)(selbstbeherrschung + (2 * sinnenschaerfe) + 1) / 3), 0, MidpointRounding.AwayFromZero);
                    var maxValue = new int[] { selbstbeherrschung * 2, sinnenschaerfe * 2 }.Min();

                    character.Talentliste.Add(new Talent
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
            }
        }

        /// <summary>
        /// Adds custom talents to the talents list if enabled
        /// </summary>
        private static void AddCustomTalents(CharacterModel character)
        {
            if (Ioc.Default.GetService<SettingsViewModel>().Setting.AdditionalTrials)
            {
                if (character.Talentliste != null)
                {
                    // Heimlichkeit
                    if (character.Talentliste.Any(t => t.Name == "Sinnenschärfe") &&
                        character.Talentliste.Any(t => t.Name == "Sich verstecken") &&
                        character.Talentliste.Any(t => t.Name == "Schleichen"))
                    {
                        var schleichen = character.Talentliste.Single(t => t.Name == "Schleichen").Value;
                        var sichverstecken = character.Talentliste.Single(t => t.Name == "Sich verstecken").Value;
                        var sinnenschaerfe = character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;

                        int talentValue = (int)Math.Round(((double)(sinnenschaerfe + sichverstecken + schleichen) / 3), 0, MidpointRounding.AwayFromZero);
                        var maxValue = new int[] { sinnenschaerfe * 2, sichverstecken * 2, schleichen * 2 }.Min();

                        character.Talentliste.Add(new Talent
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
                    if (character.Talentliste.Any(t => t.Name == "Orientierung") &&
                        character.Talentliste.Any(t => t.Name == "Sinnenschärfe") &&
                        character.Talentliste.Any(t => t.Name == "Selbstbeherrschung"))
                    {
                        var orientierung = character.Talentliste.Single(t => t.Name == "Orientierung").Value;
                        var selbstbeherrschung = character.Talentliste.Single(t => t.Name == "Selbstbeherrschung").Value;
                        var sinnenschaerfe = character.Talentliste.Single(t => t.Name == "Sinnenschärfe").Value;

                        int talentValue = (int)Math.Round(((double)(orientierung + (sinnenschaerfe * 2) + (selbstbeherrschung * 3)) / 6), 0, MidpointRounding.AwayFromZero);
                        var maxValue = new int[] { orientierung * 2, sinnenschaerfe * 2, selbstbeherrschung * 2 }.Min();

                        character.Talentliste.Add(new Talent
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

                    // Sinnenschärfe FF + KL
                    if (character.Talentliste.Any(t => t.Name == "Sinnenschärfe"))
                    {
                        Talent talent = character.Talentliste.Single(t => t.Name == "Sinnenschärfe");

                        character.Talentliste.Add(new Talent
                        {
                            Name = "Sinnenschärfe FF",
                            Probe = new Probe
                            {
                                Attribut1 = AttributType.Klugheit,
                                Attribut2 = AttributType.Intuition,
                                Attribut3 = AttributType.Fingerfertigkeit
                            },
                            Value = talent.Value,
                            Gruppe = TalentGruppe.Koerper
                        });

                        character.Talentliste.Add(new Talent
                        {
                            Name = "Sinnenschärfe KL",
                            Probe = new Probe
                            {
                                Attribut1 = AttributType.Klugheit,
                                Attribut2 = AttributType.Intuition,
                                Attribut3 = AttributType.Klugheit
                            },
                            Value = talent.Value,
                            Gruppe = TalentGruppe.Koerper
                        });
                    }

                    // Fährtensuchen IN
                    if (character.Talentliste.Any(t => t.Name == "Fährtensuchen"))
                    {
                        Talent talent = character.Talentliste.Single(t => t.Name == "Fährtensuchen");

                        character.Talentliste.Add(new Talent
                        {
                            Name = "Fährtensuchen IN",
                            Probe = new Probe
                            {
                                Attribut1 = AttributType.Klugheit,
                                Attribut2 = AttributType.Intuition,
                                Attribut3 = AttributType.Intuition
                            },
                            Value = talent.Value,
                            Gruppe = TalentGruppe.Natur
                        });
                    }

                    // Geistiger Widerstand
                    if (character.Talentliste.Any(t => t.Name == "Selbstbeherrschung"))
                    {
                        Talent talent = character.Talentliste.Single(t => t.Name == "Selbstbeherrschung");

                        character.Talentliste.Add(new Talent
                        {
                            Name = "Geistiger Widerstand",
                            Probe = new Probe
                            {
                                Attribut1 = AttributType.Mut,
                                Attribut2 = AttributType.Mut,
                                Attribut3 = AttributType.Klugheit
                            },
                            Value = talent.Value,
                            Gruppe = TalentGruppe.Wissen
                        });
                    }

                    // Ausdauern klettern
                    if (character.Talentliste.Any(t => t.Name == "Klettern"))
                    {
                        Talent talent = character.Talentliste.Single(t => t.Name == "Klettern");

                        character.Talentliste.Add(new Talent
                        {
                            Name = "Ausdauern klettern",
                            Probe = new Probe
                            {
                                Attribut1 = AttributType.Mut,
                                Attribut2 = AttributType.Gewandtheit,
                                Attribut3 = AttributType.Konstitution
                            },
                            Value = talent.Value,
                            Gruppe = TalentGruppe.Koerper
                        });
                    }

                    // Langlauf
                    if (character.Talentliste.Any(t => t.Name == "Athletik"))
                    {
                        Talent talent = character.Talentliste.Single(t => t.Name == "Athletik");

                        character.Talentliste.Add(new Talent
                        {
                            Name = "Langlauf",
                            Probe = new Probe
                            {
                                Attribut1 = AttributType.Gewandtheit,
                                Attribut2 = AttributType.Konstitution,
                                Attribut3 = AttributType.Konstitution
                            },
                            Value = talent.Value,
                            Gruppe = TalentGruppe.Koerper
                        });
                    }
                }
            }
        }

        /// <summary>
        /// convert a given character xml document to a CharacterModel object
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static CharacterModel XmlToCharacterModel(XDocument doc)
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
                                Mod = (int)attribut.Attribute("mod"),
                                Type = MapStringToAttributType((string)attribut.Attribute("name"))
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

            FindJagdwaffenTaW(character);
            AddMetaTalents(character);
            AddCustomTalents(character);
            CalcSuccessChances(character);

            //#if DEBUG
            //            Directory.CreateDirectory(@"c:\temp\");
            //            var path = $@"c:\temp\{character.Name}.xml";
            //            doc.Save(path);
            //#endif

            return character;
        }
    }
}