using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace Recomocimiento_Facial
{
    public partial class Form1 : Form
    {
        int con = 0;
        Image<Bgr, byte> currentFrame;
        Capture Grabar;
        HaarCascade face;


        Image<Gray, byte> result = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainIngImages = new List<Image<Gray, byte>>();
        List<string> Labels = new List<string>();
        List<string> NombrePersonas = new List<string>();


        public static
            MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.3d, 0.3d);


        int contTrain, numLabels, t;
        string Nombre;


        DataGridView d = new DataGridView();



        public Form1()
        {
            InitializeComponent();

            face = new HaarCascade("haarcascade_frontalface_alt_tree.xml");
            try
            {
                MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.3d, 0.3d);
                ClsDA.Consultar(d);

                string[] labels = ClsDA.Nombre;


                numLabels = ClsDA.TotalDeRostros;
                contTrain = numLabels;

                for(int i=0; i<numLabels; i++)
                {
                    con = i;
                    Bitmap bmp = new Bitmap(ClsDA.convertBinaryToImg(con));

                    trainIngImages.Add(new Image<Gray, byte>(bmp));
                    Labels.Add(labels[i]);
                }


            }
            catch(Exception)
            {
                MessageBox.Show("sin rostros para cargar");
            }
        }

        private void FrameGrabar(Object sender,EventArgs e)
        {
            Cantidad.Text = "0";
            NombrePersonas.Add("");

            try
            {
                currentFrame = Grabar.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);
                gray = currentFrame.Convert<Gray, byte>();
                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.5, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                foreach(MCvAvgComp R in RostrosDetectados[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(R.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(R.rect, new Bgr(Color.Green), 1);

                    if (trainIngImages.ToArray().Length != 0)
                    {
                        MCvTermCriteria criteria = new MCvTermCriteria(contTrain, 0.88);
                        EigenObjectRecognizer recogida = new EigenObjectRecognizer(trainIngImages.ToArray(), Labels.ToArray(), ref criteria);

                        var fa = new Image<Gray, byte>[trainIngImages.Count];

                        Nombre = recogida.Recognize(result);

                        currentFrame.Draw(Nombre, ref font, new Point(R.rect.X - 2, R.rect.Y - 2), new Bgr(Color.Green));
                    }

                    NombrePersonas[t - 1] = Nombre;
                    NombrePersonas.Add("");

                    Cantidad.Text = RostrosDetectados[0].Length.ToString();
                }
                t = 0;
                imageBox1.Image = currentFrame;
                Nombre = "";
                NombrePersonas.Clear();

            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message);
            }
        }

        private void reconocer()
        {
            try
            {
                Grabar = new Capture();
                Grabar.QueryFrame();
                Application.Idle += new EventHandler(FrameGrabar);
            }
            catch(Exception Error)
            {
                MessageBox.Show(Error.Message);
            }
        }

        private void DetenerReconocer()
        {
            try
            {
                Application.Idle += new EventHandler(FrameGrabar);
                Grabar.Dispose();
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            reconocer();
        }

        private void Registrar_Click(object sender, EventArgs e)
        {
            Registro f = new Registro();
            DetenerReconocer();
            this.Hide();
            f.ShowDialog();
            this.Visible = true;
            reconocer();
        }
    }

}
