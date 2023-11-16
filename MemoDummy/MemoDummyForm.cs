using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoDummy
{
    public partial class MemoDummyForm : Form
    {
        public MemoDummyForm()
        {
            InitializeComponent();
        }

        private void MemoDummyForm_Load(object sender, EventArgs e)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types.Where(t => t.IsSubclassOf(typeof(AbstractMemoScript))))
            {
                object obj = Activator.CreateInstance(type);
                lbScripts.Items.Add(obj);
            }
        }

        private AbstractMemoScript script;
        private void LbScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            script = lbScripts.SelectedItem as AbstractMemoScript;
            propertyGrid1.SelectedObject = script;
            btnRun.Enabled = script != null;
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (script != null)
            {
                timer1.Enabled = true;
                var sched = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Factory.StartNew(() => script.Run()).ContinueWith(task =>
                {
                    timer1.Enabled = false;
                    propertyGrid1.Refresh();
                }, sched);
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            script?.Stop();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            propertyGrid1.Refresh();
        }
    }
}