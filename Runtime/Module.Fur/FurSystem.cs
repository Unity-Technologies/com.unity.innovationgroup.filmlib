namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public class FurSystem
    {
        static FurSystem m_Instance;
        static public FurSystem instance
        {
            get
            {
                if(m_Instance == null)
                    m_Instance = new FurSystem();
                return m_Instance;
            }
        }

        private const int kDefaultShellCount = 8;
        public int ShellCount
        {
            get
            {
                // TODO: set
                return kDefaultShellCount;
            }
        }

        
    }
}