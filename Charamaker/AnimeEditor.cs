using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Charamaker3;
using Charamaker3.CharaModel;
using Component = Charamaker3.Component;
namespace Charamaker
{
    public partial class AnimeEditor : Form
    {
        Charamaker charamaker;
        FileInfo? preinfo = null;
        World w;
        Camera cam;
        bool started = false;

        float timer = 0;
        AnimeLoader anm;
        public AnimeEditor(Charamaker c)
        {
            this.charamaker = c;
            InitializeComponent();
            cam = charamaker.display.makeCamera(new ColorC(0, 0, 0, 0));
            newWorld();
        }
        void newWorld() 
        {

            cam.watchRect.remove();
            w = new World();
            charamaker.display.removeCamera(cam);
            cam = charamaker.display.makeCamera(new ColorC(0, 0, 0, 0));
            cam.watchRect.add(w);
        }

        private void AnimeEditor_Load(object sender, EventArgs e)
        {
            SetText(new DataSaver());
        }
        public void SetText(DataSaver d)
        {
            SetText(d.getData().Replace("\n", Environment.NewLine));
        }
        public void SetText(string d)
        {
            MessageBox.Text = d.Replace("\n", Environment.NewLine);
            MessageBox.Text += Environment.NewLine + "******************************" + Environment.NewLine;
            MessageBox.Text += new AnimeLoader(new DataSaver()).ToSave().getData().Replace("\n", Environment.NewLine);

        }

        private void tiked(object sender, EventArgs e)
        {
            {
                var path = @".\animation\" + this.loadBox.Text + ".anm";

                if (!File.Exists(path))
                {
                    anm = new AnimeLoader(new DataSaver());
                }
                else
                {

                    var info = new FileInfo(path);
                    if (preinfo == null || DateTime.Compare(info.LastWriteTime, preinfo.LastWriteTime) > 0)
                    {
                        var anmD = DataSaver.loadFromPath(path, true, "");
                        anm = new AnimeLoader(anmD);
                        preinfo = info;
                        anmDChanged();
                        timer = TimeBar.Value / 10f;
                        w.update(timer);
                    }
                }

            }
            if (timer < anm.maxtime && started)
            {
                var ntimer = Mathf.min(timer + 1 * ((float)SpeedUd.Value), anm.maxtime);
                w.update(ntimer - timer);
                timer = ntimer;
                this.TimeBar.Value = (int)(timer * 10);
                if (timer == anm.maxtime)
                {
                    started = false;
                }
            }
            timeLabel.Text = timer + " / " + anm.maxtime;
            if (started) 
            {
                timeLabel.Text += " is playing";
            }
            w.update(0);
        }
        void anmDChanged()
        {

            newWorld();
            anm.MakeAnime(w.staticEntity);
            {
                TimeBar.Maximum = (int)(anm.maxtime * 10);
            }
            Debug.WriteLine("animchange!");
        }

        private void shown(object sender, EventArgs e)
        {

        }

        private void close(object sender, FormClosedEventArgs e)
        {
            charamaker.display.removeCamera(cam);
        }

        private void TimeBar_Scroll(object sender, EventArgs e)
        {
            if (!started)
            {
                anmDChanged();
                timer = TimeBar.Value / 10f;
                w.update(timer);
            }
        }

        private void PlayB_Click(object sender, EventArgs e)
        {
            if (!started )
            {
                if (timer < anm.maxtime)
                {
                    started = true;
                }
                else
                {
                    anmDChanged();
                    started = true;
                    timer = 0;
                }
            }
            else 
            {
                started = false;
            }
        }

        private void MessageBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void CheckB_Click(object sender, EventArgs e)
        {
            SetText(anm.Check().Replace("\n",Environment.NewLine));
        }
    }
}
