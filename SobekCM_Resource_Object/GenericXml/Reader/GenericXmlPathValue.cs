namespace SobekCM.Resource_Object.GenericXml.Reader
{
    public class GenericXmlPathValue
    {
        public GenericXmlPath Path { get; set; }

        public string Value { get; set; }

        public GenericXmlPathValue(GenericXmlPath Path, string Value)
        {
            this.Path = Path;
            this.Value = Value;
        }

        /// <summary> Overrides the default method and returns a <see cref="System.String" /> that represents this instance </summary>
        public override string ToString()
        {
            return Path + " --> " + Value;
        }
    }
}
