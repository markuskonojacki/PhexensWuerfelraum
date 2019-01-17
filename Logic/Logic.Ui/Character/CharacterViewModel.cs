using GalaSoft.MvvmLight.Command;
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