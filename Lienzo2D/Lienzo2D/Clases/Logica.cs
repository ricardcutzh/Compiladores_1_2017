using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Interpreter;
using Irony.Ast;
using Lienzo2D.Analizador;

namespace Lienzo2D.Clases
{

    class Logica
    {
        #region atributos
        List<Variable> variables = new List<Variable>();
        string ambito;
        List<ErrorEnAnalisis> errores = new List<ErrorEnAnalisis>();
        #endregion

        #region Constructores   
        public Logica(List<Variable> vars, string ambito)
        {
            this.variables = vars;
            this.ambito = ambito;
        }
        #endregion

        #region Evaluacion de Logica
        public Object EvaluaLogica(ParseTreeNode raiz)
        {
            string Inicio = raiz.ToString();
            ParseTreeNode[] hijos = null;
            if (raiz.ChildNodes.Count > 0)
            {
                hijos = raiz.ChildNodes.ToArray();
            }
            switch (Inicio)
            {
                #region EXPLOGICA
                case "CONDICIONES":
                    {
                        if (raiz.ChildNodes.Count() == 1)//::= EXPLOGICA
                        {
                            EvaluaLogica(hijos[0]);
                        }
                        break;
                    }
                case "EXPLOGICA":
                    {
                        if (raiz.ChildNodes.Count() == 1)//::= B
                        {
                            EvaluaLogica(hijos[0]);
                        }
                        break;
                    }
                case "B":
                    {
                        if (raiz.ChildNodes.Count() == 1)//::= C
                        {
                            EvaluaLogica(hijos[0]);
                        }
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            string operando = hijos[1].ToString().Replace(" (Key symbol)", "");
                            if (operando == "||")//OR
                            {

                            }
                            if (operando == "!||")//NOR
                            {

                            }
                            if (operando == "&|")//XOR
                            {

                            }
                        }
                        break;
                    }
                case "C":
                    {
                        if (raiz.ChildNodes.Count() == 1)//::= D
                        {
                            EvaluaLogica(hijos[0]);
                        }
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            string operando = hijos[1].ToString().Replace(" (Key symbol)", "");
                            if (operando == "&&")//AND
                            {

                            }
                            if (operando == "!&&")//NAND
                            {

                            }
                        }
                        break;
                    }
                case "D":
                    {
                        if (raiz.ChildNodes.Count() == 1)//::= RELACIONALES
                        {
                            try
                            {
                                bool prueba = (Boolean)EvaluaLogica(hijos[0]);
                                return prueba;
                            }
                            catch
                            {
                                return null;
                            }
                        }
                        if (raiz.ChildNodes.Count() == 2)//::= not RELACIONALES
                        {
                            try
                            {
                                bool prueba = (Boolean)EvaluaLogica(hijos[0]);
                                return !prueba;
                            }
                            catch
                            {

                            }
                        }
                        break;
                    }

                #endregion
                #region RELACIONALES
                case "RELACIONALES":
                    {
                        if (raiz.ChildNodes.Count() == 1)//::= A
                        {
                           return EvaluaLogica(hijos[0]);
                        }
                        break;
                    }
                case "A":
                    {
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            string operando = hijos[1].ToString().Replace(" (Key symbol)", "");
                            Elemento a = (Elemento)EvaluaLogica(hijos[0]);
                            Elemento b = (Elemento)EvaluaLogica(hijos[2]);
                            if (a != null && b != null)
                            {
                                if (a.tipo == "boolean" || b.tipo == "boolean")
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Operador: (" + operando + ") No se puede Aplicar entre Tipo: " + a.tipo + " y " + b.tipo, "Error Semantico", hijos[1].Token.Location.Line, hijos[1].Token.Location.Column);
                                    this.errores.Add(error);
                                }
                                else
                                {
                                    int valor1 = Convert.ToInt32(a.valor);
                                    int valor2 = Convert.ToInt32(b.valor);
                                    return evaluarelacionales(valor1, valor2, operando);//RETORNO FALSO O VERDADERO
                                }
                            }
                        }
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            Expresion ele1 = new Expresion("boolean", this.variables, this.ambito);
                            Elemento elem = (Elemento)ele1.recorre_expresion(hijos[0]);
                            Expresion ele2 = new Expresion("entero", this.variables, this.ambito);
                            Elemento elem2 = (Elemento)ele2.recorre_expresion(hijos[0]);
                            if (elem != null)
                            {

                                return ConvertElementToBool(elem);
                            }
                            if (elem2 != null)
                            {
                                return elem2;
                            }
                            if (elem == null && elem2 == null)
                            {
                                foreach (ErrorEnAnalisis r in ele1.getErroresSemanticos())
                                {
                                    this.errores.Add(r);
                                }
                                foreach (ErrorEnAnalisis s in ele2.getErroresSemanticos())
                                {
                                    this.errores.Add(s);
                                }
                            }
                        }
                        break;
                    }
                    #endregion
            }
            return null;
        }
        #endregion


        #region Funciones Booleanas
        private bool evaluarelacionales(int valor1, int valor2, string operando)
        {
            switch (operando)
            {
                case "=="://IGUAL
                    {
                        if (valor1 == valor2)
                        {
                            return true;
                        }
                        break;
                    }
                case "!="://DIFERENTE
                    {
                        if (valor1 != valor2)
                        {
                            return true;
                        }
                        break;
                    }
                case "<"://MENOR
                    {
                        if (valor1 < valor2)
                        {
                            return true;
                        }
                        break;
                    }
                case ">"://MAYOR
                    {
                        if (valor1 > valor2)
                        {
                            return true;
                        }
                        break;
                    }
                case "<="://MENOR IGUAL
                    {
                        if (valor1 <= valor2)
                        {
                            return true;
                        }
                        break;
                    }
                case ">="://MAYOR IGUAL
                    {
                        if (valor1 >= valor2)
                        {
                            return true;
                        }
                        break;
                    }
            }
            return false;
        } 
        private bool ConvertElementToBool(Elemento elemento)
        {
            if(elemento.valor == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
