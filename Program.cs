using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.Extensions;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;

namespace Robo_Panjur {
   class Program {
      static void Main(string[] args) {

         int contador = 0;
         int ignorados = 0;
         IWebDriver ie;

         Console.WriteLine("Informe o CPF de acesso: ");
         var cpf = Console.ReadLine();
         //var cpf = "06209560970";
         Console.WriteLine("Informe a senha de acesso");
         var senha = Console.ReadLine();
         //var senha = "Daiane@21";

         Console.WriteLine("Selecione o naegador: 1 = Edge, 2 = Chrome, 3 Firefox ");
         var webDriver = Int16.Parse(Console.ReadLine());

         Console.WriteLine("Selecione uma opção: \n 1 - Atualização de informações. \n 2 - Incluir novo acompanhamento");
         int opcao = Int16.Parse(Console.ReadLine());

         if (opcao == 1) {
            Console.WriteLine("Atualização de informações");

            if (webDriver == 1) {
               ie = new EdgeDriver();
            } else if (webDriver == 2) {
               ie = new ChromeDriver();
            } else {
               ie = new FirefoxDriver();
            }

            void logar() {
               try {
                  ie.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/Principal.asp");
               } catch (Exception) {
                  logar();
               }

               Thread.Sleep(500);
               try {
                  ie.FindElement(By.Name("selEscritorio"));
               } catch (Exception) {
                  logar();
               }


               var selEscritorio = ie.FindElement(By.Name("selEscritorio"));
               selEscritorio.Click();
               selEscritorio.SendKeys("BELLIANTI PEREZ ADVOCACIA");
               Thread.Sleep(600);
               selEscritorio.SendKeys(Keys.Enter);

               Thread.Sleep(600);
               var txtLogin = ie.FindElement(By.Name("txtLogin"));
               txtLogin.Click();
               Thread.Sleep(600);
               txtLogin.SendKeys(cpf);
               Thread.Sleep(600);

               Thread.Sleep(600);
               var txtPassword = ie.FindElement(By.Name("txtPassword"));
               txtPassword.Click();
               Thread.Sleep(600);
               txtPassword.SendKeys(senha);
               Thread.Sleep(600);
               txtPassword.SendKeys(Keys.Enter);
            }

            logar();

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

                  //Acessa a pasta do contrato.
                  void acessaPasta() {
                     Thread.Sleep(500);
                     try {
                        ie.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/ProcessoExibe.asp?nat=rec&id=" + pasta);
                     } catch (Exception) {
                        executa();
                     }
                  }

                  //Prenche o form com os dados da lista.
                  void preencherForm() {
                     Thread.Sleep(500);
                     try {
                        ie.FindElement(By.Name("txtCNJ"));
                     } catch (Exception) {
                        executa();
                     }

                     Thread.Sleep(500);
                     var txtCNJ = ie.FindElement(By.Name("txtCNJ"));
                     var value = txtCNJ.GetAttribute("value");
                     if (value != cnj) {
                        var processoValue = ie.FindElement(By.Name("txtNoProcesso"));
                        processoValue.Click();
                        processoValue.SendKeys(processo);

                        var cnjValue = ie.FindElement(By.Name("txtCNJ"));
                        cnjValue.Click();
                        cnjValue.SendKeys(cnj);

                        var foroValue = ie.FindElement(By.Name("txtVara"));
                        foroValue.Click();
                        foroValue.SendKeys(foro);

                        //ie.ExecuteJavaScript("document.getElementById('txtVara').value = '" + foro + "'");

                        var radJustGratuita = ie.FindElement(By.Name("radJustGratuita"));
                        radJustGratuita.Click();
                        Thread.Sleep(200);
                        radJustGratuita.SendKeys(Keys.ArrowRight);
                        Thread.Sleep(200);

                        var txtUF = ie.FindElement(By.Name("txtUF"));
                        txtUF.Click();
                        txtUF.SendKeys(estado);
                        txtUF.SendKeys(Keys.Enter);
                        Thread.Sleep(2000);

                        try {
                           var selComarca = ie.FindElement(By.Name("selComarca"));
                           selComarca.Click();
                           selComarca.SendKeys(cidade);
                           selComarca.SendKeys(Keys.Enter);
                        } catch (Exception) {
                           executa();
                        }
                        Thread.Sleep(3000);

                        try {
                           ie.ExecuteJavaScript("document.getElementById('txtForumNome').value = '" + comarca + "'");
                        } catch (Exception) {
                           Thread.Sleep(300);
                           executa();
                        }

                        var selEmpresa = ie.FindElement(By.Name("selEmpresa"));
                        selEmpresa.Click();
                        selEmpresa.SendKeys("BANCO PAN S/A");
                        selEmpresa.SendKeys(Keys.Enter);
                        Thread.Sleep(300);

                        var txtObsAlteracao = ie.FindElement(By.Name("txtObsAlteracao"));
                        txtObsAlteracao.Click();
                        txtObsAlteracao.SendKeys(interacao);
                        Thread.Sleep(1000);

                        var submitUpdate = ie.FindElement(By.Name("submitUpdate"));
                        try {
                           Thread.Sleep(300);
                           submitUpdate.Click();
                        } catch (Exception) {
                           Thread.Sleep(300);
                           executa();
                        }


                        try {
                           var alert = ie.SwitchTo().Alert();
                           alert.Accept();
                        } catch (Exception) {
                           executa();
                        }

                        contador++;
                     } else {
                        ignorados++;
                     }

                  }

                  //Chama todos os metodos dentro do While
                  void executa() {
                     Thread.Sleep(700);
                     acessaPasta();
                     Thread.Sleep(700);
                     preencherForm();
                     Thread.Sleep(700);
                  }

                  executa();
                  Thread.Sleep(2000);
                  Console.WriteLine("Contratos encontrados já atualizados: " + ignorados);
                  Console.WriteLine("Contratos atualizados até o momento: " + contador);
                  Console.WriteLine("Ultimo contrato ativo: " + pasta + ", estado de: " + estado);
               }
            }
            Console.WriteLine("Total de contratos atualizado: " + contador);
         } else if (opcao == 2) {
            Console.WriteLine("Atualização de informações");

            if (webDriver == 1) {
               ie = new EdgeDriver();
            } else if (webDriver == 2) {
               ie = new ChromeDriver();
            } else {
               ie = new FirefoxDriver();
            }

            void logar() {
               try {
                  ie.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/Principal.asp");
               } catch (Exception) {
                  logar();
               }

               Thread.Sleep(500);
               try {
                  ie.FindElement(By.Name("selEscritorio"));
               } catch (Exception) {
                  logar();
               }


               var selEscritorio = ie.FindElement(By.Name("selEscritorio"));
               selEscritorio.Click();
               selEscritorio.SendKeys("BELLIANTI PEREZ ADVOCACIA");
               Thread.Sleep(500);
               selEscritorio.SendKeys(Keys.Enter);

               Thread.Sleep(500);
               var txtLogin = ie.FindElement(By.Name("txtLogin"));
               txtLogin.Click();
               Thread.Sleep(500);
               txtLogin.SendKeys(cpf);
               Thread.Sleep(500);

               Thread.Sleep(500);
               var txtPassword = ie.FindElement(By.Name("txtPassword"));
               txtPassword.Click();
               Thread.Sleep(500);
               txtPassword.SendKeys(senha);
               Thread.Sleep(500);
               txtPassword.SendKeys(Keys.Enter);
            }

            logar();

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


                  //Acessa a pasta do contrato.
                  void acessaPasta() {
                     Thread.Sleep(500);
                     try {
                        ie.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/ProcessoExibe.asp?nat=rec&id=" + pasta);
                     } catch (Exception) {
                        executa();
                     }
                  }

                  //Prenche o form com os dados da lista.
                  void preencherForm() {
                     Thread.Sleep(500);
                     try {
                        ie.FindElement(By.Name("txtCNJ"));
                     } catch (Exception) {
                        executa();
                     }

                     try {
                        Thread.Sleep(500);
                        var txtProcInfo = ie.FindElement(By.Id("txtProcInfo"));
                        txtProcInfo.SendKeys(interacao);
                     } catch (Exception) {

                        executa();
                     }


                     try {
                        Thread.Sleep(500);
                        var btoAcomp = ie.FindElement(By.Id("btoAcomp"));
                        btoAcomp.Click();
                     } catch (Exception) {

                        executa();
                     }


                     contador++;
                  }

                  //Chama todos os metodos dentro do While
                  void executa() {
                     Thread.Sleep(700);
                     acessaPasta();
                     Thread.Sleep(700);
                     preencherForm();
                     Thread.Sleep(700);
                  }

                  executa();
                  Thread.Sleep(2000);
                  Console.WriteLine("Contratos encontrados já atualizados: " + ignorados);
                  Console.WriteLine("Contratos atualizados até o momento: " + contador);
                  Console.WriteLine("Ultimo contrato ativo: " + pasta + ", estado de: " + estado);
               }
            }
            Console.WriteLine("Total de contratos atualizado: " + contador);
         } else {
            Console.WriteLine("Opção invalida.");
         }

      }
   }
}