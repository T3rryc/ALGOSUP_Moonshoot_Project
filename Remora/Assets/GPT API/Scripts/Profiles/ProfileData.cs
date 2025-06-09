namespace TzarGPT
{
    [System.Serializable]
    public class ProfileData
    {
        public string model;
        public string prompt;
        public double temperature;
        public int max_tokens;
        public double top_p;
        public double frequency_penalty;
        public double presence_penalty;
        public string[] stop;
    }
}