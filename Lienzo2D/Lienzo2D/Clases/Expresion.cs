using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using System.Windows.Forms;
using Lienzo2D.Analizador;
namespace Lienzo2D.Clases
{
    class Expresion//CLASE PARA LA EVALUACIÓN DE EXPRESIONES (HACE VALIDACIÓNES SEMÁNTICAS)
    {
        string tipo;//TIPO DE LA EXPRESIÓN QUE SE ESTÁ VALUANDO
        List<Variable> vars;//LISTADO DE VARIABLES ACTUALES ANALIZADAS
        string ambito; //ambito el que me encuentro

        //LISTA DE ERRORES SEMANTICOS ENCONTRADOS EN EXPRESIONES:
        List<ErrorEnAnalisis> SemanticosExpr = new List<ErrorEnAnalisis>();

        public Expresion(String tipo)//PARA EL CONSTRUCTOR LE ENVÍO UN TÍPO
        {
            //constructor por defecto
            this.tipo = tipo;
        }

        public Expresion(String tipo, List<Variable> vars, string ambito)
        {
            this.tipo = tipo;
            this.vars = vars;
            this.ambito = ambito;
        }

        public object recorre_expresion(ParseTreeNode raiz)
        {
            string Inicio = raiz.ToString();
            ParseTreeNode[] hijos = null;
            if (raiz.ChildNodes.Count > 0)
            {
                hijos = raiz.ChildNodes.ToArray();
            }
            switch (Inicio)
            {
                case "EXPR"://PRODUCCIONES DE LAS EXPR
                    {
                        if(raiz.ChildNodes.Count() == 1)//EXPR ::= E 
                        {
                            Elemento elemento = (Elemento)recorre_expresion(hijos[0]);
                            //MessageBox.Show(valor);
                            //DEBERÍA DE RETORNAR UN ELEMENTO PARA COMPROBAR SEMANTICAMENTE LOS TIPOS
                            return elemento;
                        }
                        break;
                    }
                case "E":
                    {
                        if(raiz.ChildNodes.Count() == 1)//E::= T
                        {
                            return recorre_expresion(hijos[0]);
                        }
                        if(raiz.ChildNodes.Count() == 3) //E::= E + T | E - T 
                        {
                            Elemento a = (Elemento)recorre_expresion(hijos[0]);
                            Elemento b = (Elemento)recorre_expresion(hijos[2]);
                            Elemento c = null;
                            if (hijos[1].ToString().Contains("+"))
                            {
                                if (comprobarOperacion(a, b, "+"))
                                {
                                    if (this.tipo == "entero")
                                     {
                                        int num1 = Convert.ToInt32(a.valor);
                                        int num2 = Convert.ToInt32(b.valor);
                                        int res = num1 + num2;
                                        c = new Elemento(res.ToString(), "entero");
                                     }
                                     if (this.tipo == "doble")
                                     {
                                         double num1 = Convert.ToDouble(a.valor);
                                         double num2 = Convert.ToDouble(b.valor);
                                         double resp = num1 + num2;
                                         c = new Elemento(resp.ToString(), "doble");
                                     }
                                     if (this.tipo == "boolean")
                                     {
                                         string a1 = a.valor;
                                         string b1 = b.valor;
                                         
                                         if(operacion_booleana(a1) || operacion_booleana(b1))
                                        {
                                            c = new Elemento("true", "boolean");
                                        }
                                        else
                                        {
                                            c = new Elemento("false", "boolean");
                                        }
                                     }
                                }
                            }
                            else
                            {
                                if (comprobarOperacion(a, b, "-"))
                                {
                                    if(this.tipo == "entero")
                                    {
                                        int num1 = Convert.ToInt32(a.valor);
                                        int num2 = Convert.ToInt32(b.valor);
                                        int res = num1 - num2;
                                        c = new Elemento(res.ToString(), "entero");
                                    }
                                    if(this.tipo == "doble")
                                    {
                                        double num1 = Convert.ToDouble(a.valor);
                                        double num2 = Convert.ToDouble(b.valor);
                                        double resp = num1 - num2;
                                        c = new Elemento(resp.ToString(), "doble");
                                    }
                                }
                            }
                            return c;
                        }
                        break;
                    }
                case "T":
                    {
                        if (raiz.ChildNodes.Count() == 1)//T ::= G
                        {
                            return recorre_expresion(hijos[0]);
                        }
                        if (raiz.ChildNodes.Count() == 3) //T::= T * G | T / G 
                        {
                            Elemento a = (Elemento)recorre_expresion(hijos[0]);
                            Elemento b = (Elemento)recorre_expresion(hijos[2]);
                            Elemento c = null;
                            if (hijos[1].ToString().Contains("*"))
                            {
                                if (comprobarOperacion(a, b, "*"))
                                {
                                    if(this.tipo == "entero")
                                    {
                                        int num1 = Convert.ToInt32(a.valor);
                                        int num2 = Convert.ToInt32(b.valor);
                                        int res = num1 * num2;
                                        c = new Elemento(res.ToString(), "entero");
                                    }
                                    if(this.tipo == "doble")
                                    {
                                        double num1 = Convert.ToDouble(a.valor);
                                        double num2 = Convert.ToDouble(b.valor);
                                        double res = num1 * num2;
                                        c = new Elemento(res.ToString(), "doble");
                                    }
                                    if(this.tipo == "boolean")
                                    {
                                        string a1 = a.valor;
                                        string b1 = b.valor;
                                        if(operacion_booleana(a1) && operacion_booleana(b1))
                                        {
                                            c = new Elemento("true", "boolean");
                                        }
                                        else
                                        {
                                            c = new Elemento("false", "boolean");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (comprobarOperacion(a, b, "/"))
                                {
                                    if(this.tipo == "entero")
                                    {
                                        double num1 = Convert.ToDouble(a.valor);
                                        double num2 = Convert.ToDouble(b.valor);
                                        double res = num1 / num2;
                                        c = new Elemento(res.ToString(), "doble");
                                    }
                                    if(this.tipo == "doble")
                                    {
                                        double num1 = Convert.ToDouble(a.valor);
                                        double num2 = Convert.ToDouble(b.valor);
                                        double res = num1 / num2;
                                        c = new Elemento(res.ToString(), "doble");
                                    }
                                }
                            }
                            return c;
                        }
                        break;
                    }
                case "G":
                    {
                        if (raiz.ChildNodes.Count() == 1)//G ::= F
                        {
                            return recorre_expresion(hijos[0]);
                        }
                        if (raiz.ChildNodes.Count() == 3) //G ::= G ^ F 
                        {
                            Elemento a = (Elemento)recorre_expresion(hijos[0]);
                            Elemento b = (Elemento)recorre_expresion(hijos[2]);
                            Elemento c = null;
                            if (hijos[1].ToString().Contains("^"))
                            {
                                if (comprobarOperacion(a, b, "^"))
                                {
                                    if(this.tipo == "entero")
                                    {
                                        int num1 = Convert.ToInt32(a.valor);
                                        int num2 = Convert.ToInt32(b.valor);
                                        double res = Math.Pow(num1,num2);
                                        c = new Elemento(res.ToString(), "entero");
                                    }
                                    if(this.tipo == "doble")
                                    {
                                        double num1 = Convert.ToDouble(a.valor);
                                        double num2 = Convert.ToDouble(b.valor);
                                        double res = Math.Pow(num1, num2);
                                        c = new Elemento(res.ToString(), "doble");
                                    }
                                }
                            }
                            return c;
                        }
                        break;
                    }
                case "F":
                    {
                        if (raiz.ChildNodes.Count() == 1)//F::= identificador | entero | dec | verd | fals | caracter  | RESULTADOFUN | cadena
                        {
                            Elemento elemento = null;
                            if (hijos[0].ToString().Contains(" (cade)"))
                            {
                                String valor = hijos[0].ToString().Replace(" (cade)", "");
                                elemento = new Elemento(valor, "cadena");
                            }
                            if (hijos[0].ToString().Contains(" (ent)"))
                            {
                                String valor = hijos[0].ToString().Replace(" (ent)", "");
                                elemento = new Elemento(valor, "entero");
                            }
                            if (hijos[0].ToString().Contains(" (deci)"))
                            {
                                String valor = hijos[0].ToString().Replace(" (deci)", "");
                                elemento = new Elemento(valor, "decimal");
                            }
                            if (hijos[0].ToString().Contains(" (carac)"))
                            {
                                String valor = hijos[0].ToString().Replace(" (carac)", "");
                                elemento = new Elemento(valor, "caracter");
                            }
                            if (hijos[0].ToString().Contains(" (identificador)"))
                            {
                                //DEBO DE BUSCAR EN LAS VARIABLES PARA TRAER EL VALOR QUE ESTE YA TIENE
                                elemento = buscarValorDeVar(hijos[0].ToString().Replace(" (identificador)", ""));
                                if (elemento != null)
                                {
                                    return elemento;
                                }
                                else
                                {
                                    //ERROR DE SEMANTICA... LA VARIABLE NO EXISTE O NO ESTA DECLARADA
                                }
                            }
                            if (hijos[0].ToString().Contains("true"))
                            {
                                //String valor = hijos[0].ToString().Replace(" (true)","");
                                elemento = new Elemento("true", "boolean");
                            }
                            if (hijos[0].ToString().Contains("false"))
                            {
                                //String valor = hijos[0].ToString().Replace(" (false)", "");
                                elemento = new Elemento("false", "boolean");
                            }
                            if (hijos[0].ToString().Contains("E"))
                            {
                                return recorre_expresion(hijos[0]);
                            }
                            return elemento;
                        }
                        break;
                    }
            }
            return "";
        }

        private Elemento buscarValorDeVar(string nombre)
        {
            string valor = null;
            Elemento elemento = null;
            foreach(Variable v in vars)
            {
                if(v.nombre == nombre && v.ambito == this.ambito)
                {
                    valor = v.valor;
                    elemento = new Elemento(valor, v.tipo);
                    break;
                }
                else if (v.nombre == nombre && v.esGlobal == true)
                {
                    valor = v.valor;
                    elemento = new Elemento(valor, v.tipo);
                    break;
                }

            }
            return elemento;
        }

        private bool comprobarOperacion(Elemento a, Elemento b, String operando)
        {
            bool retorno = false;
            switch (operando)
            {
                case "+":
                    {
                        if(a.tipo == "entero" && b.tipo == "entero")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "doble" && b.tipo == "doble")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "cadena" && b.tipo == "cadena")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "boolean" && b.tipo == "boolean")
                        {
                            retorno = true;
                        }
                        break;
                    }
                case "-":
                    {
                        if(a.tipo == "entero" && b.tipo == "entero")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "doble" && b.tipo == "doble")
                        {
                            retorno = true;
                        }
                        break;
                    }
                case "*":
                    {
                        if(a.tipo == "entero" && b.tipo == "entero")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "doble" && b.tipo == "doble")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "boolean" && b.tipo == "boolean")
                        {
                            retorno = true;
                        }
                        break;
                    }
                case "/":
                    {
                        if(a.tipo == "entero" && b.tipo == "entero")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "doble" && b.tipo == "doble")
                        {
                            retorno = true;
                        }
                        break;
                    }
                case "^":
                    {
                        if(a.tipo == "entero" && b.tipo == "entero")
                        {
                            retorno = true;
                        }
                        if(a.tipo == "doble" && b.tipo == "doble")
                        {
                            retorno = true;
                        }
                        break;
                    }
            }
            return retorno;
        }

        private bool operacion_booleana(string valor)
        {
            if (valor.Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private string operacion_boolInv(bool valor)
        {
            if (valor)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }
    }
}
