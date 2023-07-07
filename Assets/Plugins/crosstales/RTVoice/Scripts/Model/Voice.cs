using UnityEngine;

namespace Crosstales.RTVoice.Model
{
   /// <summary>Model for a voice.</summary>
   [System.Serializable]
   public class Voice
   {
      #region Variables

      /// <summary>Name of the voice.</summary>
      [Tooltip("Name of the voice.")] public string Name;

      /// <summary>Culture of the voice.</summary>
      [Tooltip("Culture of the voice voice."), SerializeField] private string culture;

      /// <summary>Description of the voice.</summary>
      [Tooltip("Description of the voice.")] public string Description;

      /// <summary>Gender of the voice.</summary>
      [Tooltip("Gender of the voice.")] public Crosstales.RTVoice.Model.Enum.Gender Gender;

      /// <summary>Age of the voice.</summary>
      [Tooltip("Age of the voice.")] public string Age;

      /// <summary>Identifier of the voice.</summary>
      [Tooltip("Identifier of the voice.")] public string Identifier = string.Empty;

      /// <summary>Vendor of the voice.</summary>
      [Tooltip("Vendor of the voice.")] public string Vendor = string.Empty;

      /// <summary>Sample rate in Hz of the voice.</summary>
      [Tooltip("Sample rate in Hz of the voice.")] public int SampleRate;

      /// <summary>Is the voice neural?</summary>
      [Tooltip("Is the voice neural?.")] public bool isNeural;

      #endregion


      #region Properties

      /// <summary>Culture of the voice (ISO 639-1).</summary>
      public string Culture
      {
         get => culture;

         set
         {
            if (value != null)
               culture = value.Trim().Replace('_', '-');
         }
      }

      /// <summary>Language of the voice.</summary>
      public SystemLanguage Language => Crosstales.RTVoice.Util.Helper.ISO639ToLanguage(Culture);

      /// <summary>Simplified culture of the voice.</summary>
      [System.Xml.Serialization.XmlIgnoreAttribute]
      public string SimplifiedCulture => culture.Replace("-", string.Empty);

      #endregion


      #region Constructors

      /// <summary>Default.</summary>
      public Voice()
      {
         //empty
      }

      /// <summary>Instantiate the class.</summary>
      /// <param name="name">Name of the voice.</param>
      /// <param name="description">Description of the voice.</param>
      /// <param name="gender">Gender of the voice.</param>
      /// <param name="age">Age of the voice.</param>
      /// <param name="culture">Culture of the voice.</param>
      /// <param name="id">Identifier of the voice (optional).</param>
      /// <param name="vendor">Vendor of the voice (optional).</param>
      /// <param name="sampleRate">Sample rate in Hz of the voice (optional).</param>
      /// <param name="neural">Is the voice neural (optional).</param>
      public Voice(string name, string description, Crosstales.RTVoice.Model.Enum.Gender gender, string age, string culture, string id = "", string vendor = "unknown", int sampleRate = 0, bool neural = false)
      {
         Name = name;
         Description = description;
         Gender = gender;
         Age = age;
         Culture = culture;
         Identifier = id;
         Vendor = vendor;
         SampleRate = sampleRate;
         isNeural = neural;
      }

      #endregion


      #region Overridden methods

      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         Voice o = (Voice)obj;

         return Name == o.Name &&
                Description == o.Description &&
                Gender == o.Gender &&
                Age == o.Age &&
                Identifier == o.Identifier &&
                Vendor == o.Vendor &&
                SampleRate == o.SampleRate &&
                isNeural == o.isNeural;
      }

      public override int GetHashCode()
      {
         int hash = 0;

         if (Name != null)
            hash += Name.GetHashCode();
         if (Description != null)
            hash += Description.GetHashCode();
         hash += (int)Gender * 17;
         if (Age != null)
            hash += Age.GetHashCode();
         if (Identifier != null)
            hash += Identifier.GetHashCode();
         if (Vendor != null)
            hash += Vendor.GetHashCode();
         hash += SampleRate * 17;

         return hash;
      }


      public override string ToString()
      {
         return $"{Name} ({Culture}, {Gender})";
      }

      /*
      public override string ToString()
      {
          System.Text.StringBuilder result = new System.Text.StringBuilder();

          result.Append(GetType().Name);
          result.Append(Util.Constants.TEXT_TOSTRING_START);

          result.Append("Name='");
          result.Append(Name);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Description='");
          result.Append(Description);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Gender='");
          result.Append(Gender);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Age='");
          result.Append(Age);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Culture='");
          result.Append(Culture);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Identifier='");
          result.Append(Identifier);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);
          
          result.Append("Vendor='");
          result.Append(Vendor);
          //result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Version='");
          result.Append(Version);
          //result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

          result.Append(Util.Constants.TEXT_TOSTRING_END);

          return result.ToString();
      }
      */

      #endregion
   }
}
// © 2015-2023 crosstales LLC (https://www.crosstales.com)