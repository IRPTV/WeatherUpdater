using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using WebsiteWeather.MyDBTableAdapters;

namespace WebsiteWeather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            try
            {
                WeatherTableAdapter W_Ta = new WeatherTableAdapter();
                MyDB.WeatherDataTable W_Dt = W_Ta.Select_Cities();
                string Url = "http://api.openweathermap.org/data/2.5/group?id=";
                for (int i = 0; i < W_Dt.Rows.Count; i++)
                {
                    Url += W_Dt[i]["CityId"].ToString() + ",";
                }
                Url += "&lang=sp&APPID=b85d42ec142e5181939121154dacab88";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();
                stream.Dispose();
                reader.Dispose();
                W RvLst = JsonConvert.DeserializeObject<W>(result);

                for (int i = 0; i < RvLst.list.Count; i++)
                {
                    W_Ta.Update_City(RvLst.list[i].main.temp_min.ToString(),
                      RvLst.list[i].main.temp_max.ToString(),
                      RvLst.list[i].main.temp.ToString(),
                      RvLst.list[i].weather[0].main,
                      RvLst.list[i].weather[0].description,
                      RvLst.list[i].weather[0].icon,
                      RvLst.list[i].id.ToString());
                }
                label1.Text = DateTime.Now.ToString();
            }
            catch 
            {}

            timer1.Enabled = true;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }
    }
}
