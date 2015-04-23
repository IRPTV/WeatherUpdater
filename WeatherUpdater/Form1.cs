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
using WebsiteWeatherForcats;

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
                #region Current
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
                #endregion


                #region ForeCast
                for (int i = 0; i < W_Dt.Rows.Count; i++)
                {
                    string UrlForeCast = "http://api.openweathermap.org/data/2.5/forecast/daily?cnt=2&id=";
                    UrlForeCast += W_Dt[i]["CityId"].ToString() + "&lang=sp&APPID=b85d42ec142e5181939121154dacab88";
                    HttpWebRequest requestForeCast = (HttpWebRequest)WebRequest.Create(UrlForeCast);
                    WebResponse responseForeCast = requestForeCast.GetResponse();
                    Stream streamForeCast = responseForeCast.GetResponseStream();
                    StreamReader readerForeCast = new StreamReader(streamForeCast);
                    var resultForeCast = readerForeCast.ReadToEnd();
                    streamForeCast.Dispose();
                    readerForeCast.Dispose();
                    F RvLstForeCast = JsonConvert.DeserializeObject<F>(resultForeCast);
                    W_Ta.Update_Forecast(RvLstForeCast.list[0].temp.min.ToString(),
                        RvLstForeCast.list[0].temp.max.ToString(),
                        RvLstForeCast.list[1].temp.min.ToString(),
                        RvLstForeCast.list[1].temp.max.ToString(),
                        RvLstForeCast.city.id.ToString());
                }


                #endregion




                label1.Text = DateTime.Now.ToString();
            }
            catch
            { }

            timer1.Enabled = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }
    }
}
