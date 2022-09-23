using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baitap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string ParseAccessToken(string content) => (string)JObject.Parse(content).SelectToken("access_token");
        private string ParseContractId(string content) => (string)JObject.Parse(content).SelectToken("access_token");
        private void button1_Click(object sender, EventArgs e)
        {
            var client = new RestClient("https://apigateway-econtract-staging.vnptit3.vn/auth-service/oauth/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = new
            {
                grant_type = "client_credentials",
                client_id = client_id.Text,
                client_secret = client_secret.Text
            };
            var jsonBody = JsonConvert.SerializeObject(body);

            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            access_token.Text = ParseAccessToken(response.Content);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            var rs = file.ShowDialog();
            if (rs == DialogResult.OK)
            {
                var client = new RestClient("https://apigateway-econtract-staging.vnptit3.vn/esolution-service/contracts/create-draft-from-file-raw");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddHeader("Authorization", "Bearer " + access_token.Text);
                var customBody = new
                {
                    email = "lsnganto@gmail.com",
                    sdt = "+12064563059",
                    userType = "CONSUMER",
                    ten = "Huan nc",
                    noiCap = "",
                    tenToChuc = "to chuc huannc",
                    mst = "0889509966",
                    loaiGtId = "1",
                    soDkdn = "",
                    ngayCapSoDkdn = "2021-12-22",
                    noiCapDkkd = ""

                };
                var contractBody = new
                {
                    autoRenew = "true",
                    callbackUrl = "test url",
                    contractValue = "20000",
                    creationNote = "",
                    endDate = "2022-11-17",
                    sequence = 1,
                    signFlow = new List<object>() {
                    new
                    {
                        signType = "APPROVE",
                        signForm = new List<string>(){
                            "OTP",
                            "EKYC",
                            "OTP_EMAIL",
                            "NO_AUTHEN",
                            "EKYC_EMAIL",
                            "USB_TOKEN",
                            "SMART_CA"
                        },
                        departmentId = "",
                        userId = "938d74f4-507d-40cf-bf0d-71d890b5a613",
                        sequence = 1,
                        limitDate = 3
                    }
                },
                    signForm = new List<string>(){
                            "NO_AUTHEN"
                        },
                    templateId = "606196ce5e3f61a09ef8ed55",
                    title = "Hop dong huannc",
                    validDate = "2022-11-17",
                    verificationType = "NONE",
                    fixedPosition = false
                };

                var jsoncustom = JsonConvert.SerializeObject(customBody);
                var jsoncontract = JsonConvert.SerializeObject(contractBody);

                request.AddParameter("customer", jsoncustom);
                request.AddParameter("contract", jsoncontract);
                request.AddFile("", file.FileName);
                request.AddParameter("fields", "{}");
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(contractId.Text))
            {
                MessageBox.Show("Chưa nhập ID hợp đồng");
                return;
            }

            var client = new RestClient($"https://apigateway-econtract-staging.vnptit3.vn/esolution-service/contracts/{contractId.Text}/submit-contract");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + access_token.Text);
            var body = @"";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(contractId.Text))
            {
                MessageBox.Show("Chưa nhập ID hợp đồng");
                return;
            }
            OpenFileDialog file = new OpenFileDialog();
            var rs = file.ShowDialog();
            if (rs == DialogResult.OK)
            {
                var client = new RestClient($"https://apigateway-econtract-staging.vnptit3.vn/esolution-service/contracts/{contractId.Text}/digital-sign");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + access_token.Text);
                request.AddFile("file", @"F:/1_001_C22TTM_35_30760.pdf");
                var body = new
                {
                    SignForm = "OTP_EMAIL",
                    name = "Nguyen cong huan",
                    taxCode = "1231231234",
                    provider = "Công Ty VNPT",
                    pathImg = "iVBORw0KGgoAAAANSUhEUgAAAZAAAAJYCAYAAABM7LCIAABym",
                    identifierCode = "1231231234",
                    phone = "0917881199",
                    email = "lsnganto@gmail.com",
                    status = "VALID",
                    signType = "APPROVAL",
                    ekycInfo = "APPROVAL"
                };
                var jsonBody = JsonConvert.SerializeObject(body);
                request.AddParameter("data", jsonBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
        }
    }
}
