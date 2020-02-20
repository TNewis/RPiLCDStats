namespace RPiLCDPiServer.Services.ConnectionService
{
    static class UpdateTags
    {
        public static Tag StaticDataTag = new Tag("Static");

        public static Tag AudioDeviceTag = new Tag("Audio");
        public static Tag AudioSpeakerTag = new Tag("Speaker");
        public static Tag AudioHeadphoneTag = new Tag("Headphone");

        public static Tag ClockTabTag = new Tag("CLOCKTab");
        public static Tag ClockTag = new Tag("CLOCK");

        public static Tag GPUTabTag = new Tag("GPU");
        public static Tag GPUNameTag = new Tag("GPUName");
        public static Tag GPULoadTag = new Tag("GPULoad");
        public static Tag GPUMemoryTag = new Tag("GPUMemory");
        public static Tag GPUMemoryTotalTag = new Tag("GPUMemoryTotal");
        public static Tag GPUTemperatureTag = new Tag("GPUTemp");

        public static Tag PCMemoryTag = new Tag("PCMemory");
        public static Tag PCMemoryTotalTag = new Tag("PCMemoryTotal");

        public static Tag CPUTabTag = new Tag("CPU");
        public static Tag CPUNameTag = new Tag("CPUName");
        public static Tag CPULoadTag = new Tag("CPULoad");
        public static Tag CPUTemperatureTag = new Tag("CPUTemp");

        public static Tag TemperatureUnitsTag = new Tag("TempUnit");
    }

    public class Tag
    {
        public string TagText;
        public string TagOpen;
        public string TagClose;

        public Tag(string tagText)
        {
            TagText = tagText;
            TagOpen = "<" + tagText + ">";
            TagClose = "</" + tagText + ">";
        }
    }
}
