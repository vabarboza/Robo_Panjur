using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Robo_Panjur {
    class Program {
        static void Main(string[] args) {

            StreamReader arquivo = new StreamReader("lista.txt");
            int contador = 0;
            int ignorados = 0;

            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader("lista.txt")) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    list = line.Split(';').ToList();

                    string pasta = list[0];
                    string processo = list[1];
                    string cnj = list[2];
                    string foro = list[3];
                    string estado = list[4];
                    string cidade = list[5];
                    string comarca = list[6];
                    string interacao = list[7];

                    IWebDriver driver = new InternetExplorerDriver();
                    driver.Navigate().GoToUrl("https://panjur.panamericano.com.br/");
                    void logar() {
                        driver.Navigate().GoToUrl("https://panjur.panamericano.com.br/");
                        var selEscritorio = driver.FindElement(By.Name("selEscritorio"));
                        selEscritorio.Click();
                        selEscritorio.SendKeys("BELLIANTI PEREZ ADVOCACIA");
                        selEscritorio.SendKeys(Keys.Enter);

                        var txtLogin = driver.FindElement(By.Name("txtLogin"));
                        txtLogin.Click();
                        txtLogin.SendKeys("08300494952");

                        var txtPassword = driver.FindElement(By.Name("txtPassword"));
                        txtPassword.Click();
                        txtPassword.SendKeys("Natalia@18");
                        txtPassword.SendKeys(Keys.Enter);

                        Thread.Sleep(2000);
                        try {
                            driver.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/ProcessoExibe.asp?nat=rec&id=" + pasta);
                        } catch (Exception) {
                            driver.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/ProcessoExibe.asp?nat=rec&id=" + pasta);
                        }

                    }

                    logar();
                    try {
                        driver.FindElement(By.Name("txtCNJ"));
                    } catch {
                        logar();
                    }

                    Thread.Sleep(2000);
                    var txtCNJ = driver.FindElement(By.Name("txtCNJ"));
                    var value = txtCNJ.GetAttribute("value");

                    if (value != cnj) {
                        driver.ExecuteJavaScript("document.getElementById('txtNoProcesso').value = '" + processo + "'");

                        driver.ExecuteJavaScript("document.getElementById('txtCNJ').value = '" + cnj + "'");

                        driver.ExecuteJavaScript("document.getElementById('txtVara').value = '" + foro + "'");

                        var radJustGratuita = driver.FindElement(By.Name("radJustGratuita"));
                        radJustGratuita.Click();
                        Thread.Sleep(200);
                        radJustGratuita.SendKeys(Keys.ArrowRight);
                        Thread.Sleep(200);

                        var txtUF = driver.FindElement(By.Name("txtUF"));
                        txtUF.Click();
                        txtUF.SendKeys(estado);
                        txtUF.SendKeys(Keys.Enter);
                        Thread.Sleep(1000);

                        var selComarca = driver.FindElement(By.Name("selComarca"));
                        selComarca.Click();
                        selComarca.SendKeys(cidade);
                        selComarca.SendKeys(Keys.Enter);
                        Thread.Sleep(3000);

                        try {
                            driver.ExecuteJavaScript("document.getElementById('txtForumNome').value = '" + comarca + "'");
                        } catch (Exception) {
                            Thread.Sleep(500);
                            driver.ExecuteJavaScript("document.getElementById('txtForumNome').value = '" + comarca + "'");
                        }


                        var selEmpresa = driver.FindElement(By.Name("selEmpresa"));
                        selEmpresa.Click();
                        selEmpresa.SendKeys("BANCO PAN S/A");
                        selEmpresa.SendKeys(Keys.Enter);
                        Thread.Sleep(300);

                        var txtObsAlteracao = driver.FindElement(By.Name("txtObsAlteracao"));
                        txtObsAlteracao.Click();
                        txtObsAlteracao.SendKeys(interacao);
                        Thread.Sleep(300);

                        driver.FindElement(By.Name("submitUpdate")).Click();

                        try {
                            var alert = driver.SwitchTo().Alert();
                            alert.Accept();
                        } catch {
                            driver.Close();
                        }

                        contador++;
                    } else {
                        ignorados++;
                    }


                    Thread.Sleep(2000);
                    Console.WriteLine("Contratos encontrados já atualizados: " + ignorados);
                    Console.WriteLine("Contratos atualizados até o momento: " + contador);
                    driver.Close();
                }
            }
        }
    }
}
