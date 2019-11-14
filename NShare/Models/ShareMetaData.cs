using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NShare.Models
{
    [Serializable]
    [XmlRoot("share")]
    public class ShareMetaData
    {
        [XmlElement("id")]
        public int id { get; set; }

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("quantity")]
        public int quantity { get; set; }

        [XmlElement("unit_price")]
        public int unit_price { get; set; }

        [XmlElement("closed_date")]
        public Nullable<System.DateTime> closed_date { get; set; }
    }


    [MetadataType(typeof(ShareMetaData))]
    public partial class Share
    {
    }
}