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
        #region Globales
        string tipo;//TIPO DE LA EXPRESIÓN QUE SE ESTÁ VALUANDO
        List<Variable> vars;//LISTADO DE VARIABLES ACTUALES ANALIZADAS
        string ambito; //ambito el que me encuentro

        List<Parametro> parametros = new List<Parametro>();

        //LISTA DE ERRORES SEMANTICOS ENCONTRADOS EN EXPRESIONES:
        List<ErrorEnAnalisis> SemanticosExpr = new List<ErrorEnAnalisis>();

        //LISTA PARA OBTENER LOS INDICES QUE ME AYUDARÁN A OBTENER LA INFORMACIÓN DEL ARREGLO
        List<int> indices = new List<int>();

        //POSICION DE EXPRESION
        int line;
        int colum;
        #endregion

        #region Constructores
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

        public Expresion(String tipo, List<Variable> vars, string ambito, List<Parametro> parametros)
        {
            this.tipo = tipo;
            this.vars = vars;
            this.ambito = ambito;
            this.parametros = parametros;
        }
        #endregion

        #region Gets
        public List<ErrorEnAnalisis> getErroresSemanticos()
        {
            return this.SemanticosExpr;//LISTA DE ERRORES SEMANTICOS ENCONTRADOS EN EXPRESIONES
        }
        #endregion

        #region Analisis de Expresion
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
                            if (elemento != null)
                            {
                                if(elemento.tipo != this.tipo)
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("No se puede asignar valor de tipo: " + elemento.tipo + " a una variable de tipo: " + this.tipo, "Error Semantico", this.line, this.colum);
                                    this.SemanticosExpr.Add(error);
                                    return null;
                                }
                            }
                            return elemento;
                            //MessageBox.Show(valor);
                            //DEBERÍA DE RETORNAR UN ELEMENTO PARA COMPROBAR SEMANTICAMENTE LOS TIPOS
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
                            if(a == null || b == null)
                            {
                                return null;
                            }
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
                                     if(this.tipo == "cadena")
                                    {
                                        string a1 = a.valor;
                                        string b1 = b.valor;
                                        c = new Elemento(a1 + b1, "cadena");
                                    }
                                }
                                else //SI LA OPERACION ENTRE A Y B NO SE PUEDE REALIZAR ES UN ERROR SEMANTICO:
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Operacion de Suma entre tipos " + a.tipo + " y " + b.tipo + " no es posible", "Error Semantico", hijos[1].Token.Location.Line, hijos[1].Token.Location.Column);
                                    this.SemanticosExpr.Add(error);
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
                                else //SI NO SE PUEDE REALIZAR ES ERROR SEMANTICO
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Operacion de Resta entre tipos " + a.tipo + " y " + b.tipo + " no es posible", "Error Semantico", hijos[1].Token.Location.Line, hijos[1].Token.Location.Column);
                                    this.SemanticosExpr.Add(error);
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
                            if(a==null || b == null)
                            {
                                return null;
                            }
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
                                else //ERROR SEMANTICO
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Operacion de Multiplicacion entre tipos " + a.tipo + " y " + b.tipo + " no es posible", "Error Semantico", hijos[1].Token.Location.Line, hijos[1].Token.Location.Column);
                                    this.SemanticosExpr.Add(error);
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
                                else//error de semantica
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Operacion de Division entre tipos " + a.tipo + " y " + b.tipo + " no es posible", "Error Semantico", hijos[1].Token.Location.Line, hijos[1].Token.Location.Column);
                                    this.SemanticosExpr.Add(error);
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
                            if(a == null || b == null)
                            {
                                return null;
                            }
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
                                else//error de semantica
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Operacion de Potencia entre tipos " + a.tipo + " y " + b.tipo + " no es posible", "Error Semantico", hijos[1].Token.Location.Line, hijos[1].Token.Location.Column);
                                    this.SemanticosExpr.Add(error);
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
                            if (!hijos[0].ToString().Equals("E"))
                            {
                                this.line = hijos[0].Token.Location.Line;
                                this.colum = hijos[0].Token.Location.Column;
                            } 
                            Elemento elemento = null;
                            if (hijos[0].ToString().Contains(" (cade)"))//::=cadena
                            {
                                String valor = hijos[0].ToString().Replace(" (cade)", "");
                                elemento = new Elemento(valor, "cadena");
                            }
                            if (hijos[0].ToString().Contains(" (ent)"))//::=entero
                            {
                                String valor = hijos[0].ToString().Replace(" (ent)", "");
                                elemento = new Elemento(valor, "entero");
                            }
                            if (hijos[0].ToString().Contains(" (deci)"))//::=decimal (doble)
                            {
                                String valor = hijos[0].ToString().Replace(" (deci)", "");
                                elemento = new Elemento(valor, "doble");
                            }
                            if (hijos[0].ToString().Contains(" (carac)"))//::=caracter
                            {
                                String valor = hijos[0].ToString().Replace(" (carac)", "");
                                elemento = new Elemento(valor, "caracter");
                            }
                            if (hijos[0].ToString().Contains(" (identificador)"))//::=identificador
                            {
                                //DEBO DE BUSCAR EN LAS VARIABLES PARA TRAER EL VALOR QUE ESTE YA TIENE
                                elemento = buscarValorDeVar(hijos[0].ToString().Replace(" (identificador)", ""));
                                if(elemento == null)
                                {
                                    //BUSCARE EL ELEMENTO EN PARAMETROS
                                    elemento = buscarValorDeVarPara(hijos[0].ToString().Replace(" (identificador)", ""));
                                }
                                if (elemento != null)
                                {
                                    return elemento;
                                }
                                else
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Variable: '" + hijos[0].ToString().Replace(" (identificador)", "") + "' No declarada o No disponible en este Ambito: "+this.ambito, "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                    this.SemanticosExpr.Add(error);
                                    //ERROR DE SEMANTICA... LA VARIABLE NO EXISTE O NO ESTA DECLARADA
                                }
                            }
                            if (hijos[0].ToString().Contains("true"))//::= true
                            {
                                //String valor = hijos[0].ToString().Replace(" (true)","");
                                elemento = new Elemento("true", "boolean");
                            }
                            if (hijos[0].ToString().Contains("false"))//::= false
                            {
                                //String valor = hijos[0].ToString().Replace(" (false)", "");
                                elemento = new Elemento("false", "boolean");
                            }
                            if (hijos[0].ToString().Equals("E"))//::= (E)//AQUI QUITE CONTAINS PRO EQUALS
                            {
                                return recorre_expresion(hijos[0]);
                            }
                            return elemento;
                        }
                        if(raiz.ChildNodes.Count() == 2)//identificador DIMENSIONES
                        {
                            this.line = hijos[0].Token.Location.Line;
                            this.colum = hijos[0].Token.Location.Line;
                            Elemento elemento = null;
                            string nombreArreglo = hijos[0].ToString().Replace(" (identificador)","");
                            recorre_expresion(hijos[1]);
                            elemento = buscarValorDeArray(nombreArreglo, this.indices);
                            indices.Clear();
                            return elemento;
                        }
                        break;
                    }
                case "DIMENSIONES":
                    {
                        if(raiz.ChildNodes.Count() == 4)
                        {
                            Elemento el = (Elemento)recorre_expresion(hijos[1]);
                            if(el != null)
                            {
                                this.indices.Add(Convert.ToInt32(el.valor));
                            }
                            recorre_expresion(hijos[2]);
                        }
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            Elemento el = (Elemento)recorre_expresion(hijos[1]);
                            if(el != null)
                            {
                                this.indices.Add(Convert.ToInt32(el.valor));
                            }
                        }
                        break;
                    }
            }
            return "";
        }
        #endregion

        #region Funciones y Metodos auxiliares
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
                    //break;
                }
                else if (v.nombre == nombre && v.esGlobal == true)
                {
                    valor = v.valor;
                    elemento = new Elemento(valor, v.tipo);
                    //break;
                }

            }
            return elemento;
        }

        private Elemento buscarValorDeVarPara(string nombre)
        {
            string valor = null;
            Elemento elemento = null;
            foreach(Parametro p in this.parametros)
            {
                if(p.nombre == nombre)
                {
                    valor = p.valor;
                    elemento = new Elemento(valor, p.tipo);
                }
            }
            return elemento;
        }

        private Elemento buscarValorDeArray(string nombre, List<int> indices)
        {
            Elemento retorno = null;
            foreach(Variable v in this.vars)
            {
                if (v.esArreglo)
                {
                    if(v.nombre == nombre)
                    {
                        if (v.Valores.Count() == 1 && indices.Count() == 1)//EN CASO DE UNA DIMENSION
                        {
                            List<int> aux = v.Valores.ElementAt(0);
                            int index = indices.ElementAt(0);
                            int maxIndex = aux.Count() - 1;//INDICE MAXIMO DEL ARREGLO
                            String valor = "";
                            if (index <= maxIndex)
                            {
                                valor = aux.ElementAt(index).ToString();
                                retorno = new Elemento(valor, "entero");
                                break;
                            }
                            else
                            {
                                ErrorEnAnalisis err = new ErrorEnAnalisis("Indice afuera de los limites del Arreglo: " + v.nombre + " en Ambito: " + this.ambito, "Error Semantico", this.line, this.colum);
                                this.SemanticosExpr.Add(err);
                            }
                        }
                    }
                }
            }
            return retorno;
        }//SOLO ARREGLOS DE UNA DIMENSION

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
        #endregion
    }
}
