﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace diNo.Xml.Schulerfolgsstatistik {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class schulerfolg {
        
        private schule[] schuleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("schule")]
        public schule[] schule {
            get {
                return this.schuleField;
            }
            set {
                this.schuleField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class schule {
        
        private jahrgangsstufe11 jahrgangsstufe11Field;
        
        private jahrgangsstufe12 jahrgangsstufe12Field;
        
        private jahrgangsstufe13 jahrgangsstufe13Field;
        
        private schuleArt artField;
        
        private string nummerField;
        
        /// <remarks/>
        public jahrgangsstufe11 jahrgangsstufe11 {
            get {
                return this.jahrgangsstufe11Field;
            }
            set {
                this.jahrgangsstufe11Field = value;
            }
        }
        
        /// <remarks/>
        public jahrgangsstufe12 jahrgangsstufe12 {
            get {
                return this.jahrgangsstufe12Field;
            }
            set {
                this.jahrgangsstufe12Field = value;
            }
        }
        
        /// <remarks/>
        public jahrgangsstufe13 jahrgangsstufe13 {
            get {
                return this.jahrgangsstufe13Field;
            }
            set {
                this.jahrgangsstufe13Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public schuleArt art {
            get {
                return this.artField;
            }
            set {
                this.artField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nummer {
            get {
                return this.nummerField;
            }
            set {
                this.nummerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class jahrgangsstufe11 {
        
        private klasse[] klasseField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("klasse")]
        public klasse[] klasse {
            get {
                return this.klasseField;
            }
            set {
                this.klasseField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class klasse {
        
        private schueler[] schuelerField;
        
        private string nummerField;
        
        private klasseAusbildungsrichtung ausbildungsrichtungField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("schueler")]
        public schueler[] schueler {
            get {
                return this.schuelerField;
            }
            set {
                this.schuelerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nummer {
            get {
                return this.nummerField;
            }
            set {
                this.nummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public klasseAusbildungsrichtung ausbildungsrichtung {
            get {
                return this.ausbildungsrichtungField;
            }
            set {
                this.ausbildungsrichtungField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class schueler {
        
        private grunddaten grunddatenField;
        
        private string nummerField;
        
        /// <remarks/>
        public grunddaten grunddaten {
            get {
                return this.grunddatenField;
            }
            set {
                this.grunddatenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nummer {
            get {
                return this.nummerField;
            }
            set {
                this.nummerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class grunddaten {
        
        private string pz_bisField;
        
        private string ausgetreten_amField;
        
        private grunddatenGeschlecht geschlechtField;
        
        private string pruefungField;
        
        private grunddatenPz_bestanden pz_bestandenField;
        
        private bool pz_bestandenFieldSpecified;
        
        private grunddatenJgst_bestanden jgst_bestandenField;
        
        /// <remarks/>
        public string pz_bis {
            get {
                return this.pz_bisField;
            }
            set {
                this.pz_bisField = value;
            }
        }
        
        /// <remarks/>
        public string ausgetreten_am {
            get {
                return this.ausgetreten_amField;
            }
            set {
                this.ausgetreten_amField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public grunddatenGeschlecht geschlecht {
            get {
                return this.geschlechtField;
            }
            set {
                this.geschlechtField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pruefung {
            get {
                return this.pruefungField;
            }
            set {
                this.pruefungField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public grunddatenPz_bestanden pz_bestanden {
            get {
                return this.pz_bestandenField;
            }
            set {
                this.pz_bestandenField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pz_bestandenSpecified {
            get {
                return this.pz_bestandenFieldSpecified;
            }
            set {
                this.pz_bestandenFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public grunddatenJgst_bestanden jgst_bestanden {
            get {
                return this.jgst_bestandenField;
            }
            set {
                this.jgst_bestandenField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    public enum grunddatenGeschlecht {
        
        /// <remarks/>
        m,
        
        /// <remarks/>
        w,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    public enum grunddatenPz_bestanden {
        
        /// <remarks/>
        ja,
        
        /// <remarks/>
        nein,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    public enum grunddatenJgst_bestanden {
        
        /// <remarks/>
        ja,
        
        /// <remarks/>
        nein,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    public enum klasseAusbildungsrichtung {
        
        /// <remarks/>
        ABU,
        
        /// <remarks/>
        GA,
        
        /// <remarks/>
        GH,
        
        /// <remarks/>
        IW,
        
        /// <remarks/>
        S,
        
        /// <remarks/>
        T,
        
        /// <remarks/>
        W,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class jahrgangsstufe12 {
        
        private klasse[] klasseField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("klasse")]
        public klasse[] klasse {
            get {
                return this.klasseField;
            }
            set {
                this.klasseField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://tempuri.org/schulerfolg_1.1", IsNullable=false)]
    public partial class jahrgangsstufe13 {
        
        private klasse[] klasseField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("klasse")]
        public klasse[] klasse {
            get {
                return this.klasseField;
            }
            set {
                this.klasseField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/schulerfolg_1.1")]
    public enum schuleArt {
        
        /// <remarks/>
        FOS,
        
        /// <remarks/>
        BOS,
    }
}
