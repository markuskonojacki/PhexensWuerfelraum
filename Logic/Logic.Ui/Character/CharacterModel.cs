using Jot;
using Jot.DefaultInitializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        [Trackable]
        public ObservableCollection<Attribut> Attribute { get; set; }

        [Trackable]
        public ObservableCollection<Ausbildung> Ausbildungen { get; set; }

        [Trackable]
        public int Behinderung { get; set; }

        [Trackable]
        public int Modifikation { get; set; }

        [Trackable]
        public string FileName { get; set; }

        [Trackable]
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

        [Trackable]
        public string Name { get; set; }

        [Trackable]
        public ObservableCollection<Sonderfertigkeit> Sonderfertigkeiten { get; set; }

        [Trackable]
        public string Stand { get; set; }

        [Trackable]
        public ObservableCollection<Talent> Talentliste { get; set; }

        public List<Talent> KoerperTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Koerper).ToList(); }
        public List<Talent> GesellschaftTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Gesellschaft).ToList(); }
        public List<Talent> NaturTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Natur).ToList(); }
        public List<Talent> WissenTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Wissen || t.Gruppe == TalentGruppe.Sprachen || t.Gruppe == TalentGruppe.Schrift).ToList(); }
        public List<Talent> HandwerkTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Handwerk).ToList(); }
        public List<Talent> MetaTalentliste { get => Talentliste?.Where(t => t.Gruppe == TalentGruppe.Meta || t.Gruppe == TalentGruppe.Gabe || t.Gruppe == TalentGruppe.Custom).ToList(); }

        [Trackable]
        public ObservableCollection<Vorteil> Vorteile { get; set; }

        [Trackable]
        public ObservableCollection<Zauber> Zauberliste { get; set; }

        #region static attributes

        public int CH { get => Attribute.Where(a => a.Type == AttributType.Charisma).SingleOrDefault().Value; }
        public int FF { get => Attribute.Where(a => a.Type == AttributType.Fingerfertigkeit).SingleOrDefault().Value; }
        public int GE { get => Attribute.Where(a => a.Type == AttributType.Gewandtheit).SingleOrDefault().Value; }
        public int IN { get => Attribute.Where(a => a.Type == AttributType.Intuition).SingleOrDefault().Value; }
        public int KK { get => Attribute.Where(a => a.Type == AttributType.Koerperkraft).SingleOrDefault().Value; }
        public int KL { get => Attribute.Where(a => a.Type == AttributType.Klugheit).SingleOrDefault().Value; }
        public int KO { get => Attribute.Where(a => a.Type == AttributType.Konstitution).SingleOrDefault().Value; }
        public int MU { get => Attribute.Where(a => a.Type == AttributType.Mut).SingleOrDefault().Value; }

        #endregion static attributes

        #endregion properties

        #region methods

        public void InitConfiguration(TrackingConfiguration configuration)
        {
            configuration
            .IdentifyAs("LastCharacter")
            .RegisterPersistTrigger(nameof(PropertyChanged));
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

        #region structs

        public class Attribut : BaseModel
        {
            public string Name { get; set; } = "";
            public AttributType Type { get; set; } = AttributType.Wildcard;
            public int Value { get; set; } = 0;
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

        public class Zauber : BaseModel
        {
            public string Name { get; set; }
            public Probe Probe { get; set; }
            public int Value { get; set; }
        }

        #endregion structs
    }
}