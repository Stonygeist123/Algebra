namespace Algebra
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Btn_Graph_Click(object sender, EventArgs e)
        {
            Hide();
            Graphing g = new();
            g.Show();
        }

        private void Btn_Calc_Click(object sender, EventArgs e)
        {
            Hide();
            Calculator g = new();
            g.Show();
        }
    }
}
