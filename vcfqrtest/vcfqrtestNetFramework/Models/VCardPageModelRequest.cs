namespace vcfqrtestNetFramework.Models
{
    public class VCardPageModelRequest
    {
        public VCard VCard { get; set; } = new VCard();
        public Setting Setting { get; set; } = new Setting();
        public bool Logo { get; set; } = false;
    }
}