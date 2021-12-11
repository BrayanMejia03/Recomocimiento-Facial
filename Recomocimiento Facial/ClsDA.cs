using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Recomocimiento_Facial
{
    class ClsDA
    {
        private static OleDbConnection cnx = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = DBRostros.accdb;");

        public static string[] Nombre;
        public static string[] Correo;
        public static string[] Telefono;
        public static string[] ID;
        public static byte[] Rostros;

        public static List<byte[]> ListaDeRostros = new List<byte[]>();
        public static int TotalDeRostros;

        public static bool GuardarImagen(string ID,string Nombre,string Correo,string Telefono, byte[] Imagen)
        {
            cnx.Open();
            OleDbCommand cmd = new OleDbCommand("INSERT INTRO Rostros(ID,Nombre,Correo,Telefono,Imagen) Values ('"+ID+","+Nombre+","+Correo+","+Telefono+"',?);");
            OleDbParameter parImagen = new OleDbParameter("@Imagen", OleDbType.VarBinary, Imagen.Length);
            parImagen.Value = Imagen;
            cmd.Parameters.Add(Imagen);
            int Resultado = cmd.ExecuteNonQuery();
            cnx.Close();

            return Convert.ToBoolean( Resultado);
        }

        public static DataTable Consultar(DataGridView Data)
        {
            cnx.Open();
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Rostros;", cnx);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Data.DataSource = dt;
            int cont = dt.Rows.Count;
            ID = new string[cont];
            Nombre = new string[cont];
            Correo = new string[cont];
            Telefono = new string[cont];

            for(int i=0; i<cont; i++)
            {
                ID[i] =  dt.Rows[i]["ID"].ToString();
                Nombre[i] = dt.Rows[i]["Nombre"].ToString();
                Correo[i] = dt.Rows[i]["Correo"].ToString();
                Telefono[i] = dt.Rows[i]["Telefono"].ToString();
                Rostros = (byte[])dt.Rows[i]["Imagen"];
                ListaDeRostros.Add(Rostros);
                
            }

            try
            {
                Data.Columns[0].Width = 60;
                Data.Columns[1].Width = 160; 
                Data.Columns[2].Width = 160;

                for (int i = 0; i < cont; i++)
                {
                    Data.Rows[i].Height = 119;

                }

            }
            catch
            {

            }

            TotalDeRostros = cont;
            cnx.Close();
            return dt;
        }

        /////
        
        public static byte[] ConvertImgToBinary(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            MemoryStream Memoria = new MemoryStream();
            bmp.Save(Memoria,ImageFormat.Bmp);

            byte[] imagen = Memoria.ToArray();

            return imagen;

        }

        public static Image convertBinaryToImg(int c)
        {
            Image Imagen;
            byte[] img = ListaDeRostros[c];
            MemoryStream Memoria = new MemoryStream(img);
            Imagen = Image.FromStream(Memoria);
            Memoria.Close();
            return Imagen;

        }
    }
}
