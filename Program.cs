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
      int contador = 0;
      int ignorados = 0;

      IWebDriver driver = new InternetExplorerDriver();

      void logar() {
        try {
          driver.Navigate().GoToUrl("https://panjur.panamericano.com.br/");
        } catch (Exception) {
          logar();
        }

        Thread.Sleep(500);
        try {
          driver.FindElement(By.Name("selEscritorio"));
        } catch (Exception) {
          logar();
        }


        var selEscritorio = driver.FindElement(By.Name("selEscritorio"));
        selEscritorio.Click();
        selEscritorio.SendKeys("BELLIANTI PEREZ ADVOCACIA");
        Thread.Sleep(500);
        selEscritorio.SendKeys(Keys.Enter);

        Thread.Sleep(500);
        var txtLogin = driver.FindElement(By.Name("txtLogin"));
        txtLogin.Click();
        Thread.Sleep(500);
        txtLogin.SendKeys("08300494952");
        Thread.Sleep(500);

        Thread.Sleep(500);
        var txtPassword = driver.FindElement(By.Name("txtPassword"));
        txtPassword.Click();
        Thread.Sleep(500);
        txtPassword.SendKeys("Natalia@19");
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
              driver.Navigate().GoToUrl("https://panjur.panamericano.com.br/Assessoria/ProcessoExibe.asp?nat=rec&id=" + pasta);
            } catch (Exception) {
              executa();
            }
          }

          //Prenche o form com os dados da lista.
          void preencherForm() {
            Thread.Sleep(500);
            try {
              driver.FindElement(By.Name("txtCNJ"));
            } catch (Exception) {
              executa();
            }

            Thread.Sleep(500);
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
              Thread.Sleep(2000);

              try {
                var selComarca = driver.FindElement(By.Name("selComarca"));
                selComarca.Click();
                selComarca.SendKeys(cidade);
                selComarca.SendKeys(Keys.Enter);
              } catch (Exception) {
                executa();
              }
              Thread.Sleep(3000);

              try {
                driver.ExecuteJavaScript("document.getElementById('txtForumNome').value = '" + comarca + "'");
              } catch (Exception) {
                Thread.Sleep(300);
                executa();
              }

              var selEmpresa = driver.FindElement(By.Name("selEmpresa"));
              selEmpresa.Click();
              selEmpresa.SendKeys("BANCO PAN S/A");
              selEmpresa.SendKeys(Keys.Enter);
              Thread.Sleep(300);

              var txtObsAlteracao = driver.FindElement(By.Name("txtObsAlteracao"));
              txtObsAlteracao.Click();
              txtObsAlteracao.SendKeys(interacao);
              Thread.Sleep(1000);

              var submitUpdate = driver.FindElement(By.Name("submitUpdate"));
              try {
                Thread.Sleep(300);
                submitUpdate.Click();
              } catch (Exception) {
                Thread.Sleep(300);
                executa();
              }


              try {
                var alert = driver.SwitchTo().Alert();
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
    }
  }
}
