using Charamaker3;
namespace DataChecker
{
    public partial class DataCheckForm : Form
    {
        DataSaver save;
        string Path="",KaniPackString = "", KaniPackEnd = "", PackNameString = ""; 
        public DataCheckForm()
        {
            InitializeComponent();

            {
                save = DataSaver.loadFromPath(@".\save", false);


                Path = save.unpackDataS("Path", "");
                Debug.WriteLine("SaveData " + save.getData());
                KaniPackString=(save.unpackDataS("KaniPackString", @""));
                KaniPackEnd=(save.unpackDataS("kaniPackEnd", @""));
                PackNameString = save.unpackDataS("PackNameString", "");

                Reload();
            }
        }

        private void Ticked(object sender, EventArgs e)
        {

        }

        void Reload() 
        {
            PathBox.Text = Path;
            DataSaver.loadFromPath(Path);
            
        }
    }
}
