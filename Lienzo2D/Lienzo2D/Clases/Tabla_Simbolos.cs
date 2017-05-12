using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using Lienzo2D.Analizador;
using System.Windows.Forms;
namespace Lienzo2D.Clases
{
    class Tabla_Simbolos
    {
        #region Lienzo
        String tipo_actual_evaluado; //TIPO DE VARIABLE ACTUAL EVALUADA

        List<Simbolo> Tabla = new List<Simbolo>(); //TABLA DE SIMBOLOS

        Stack<String> ambitos = new Stack<string>();//PILA DE AMBITOS

        ParseTreeNode raizAST;//RAÍZ DEL ARBOL CON EL QUE EMPIEZO EL RECORRIDO

        List<Variable> variables = new List<Variable>();//LISTA DE TODAS LAS VARIABLES QUE EXISTEN EN EN LIENZO ANALIZADO

        List<Procedimiento> procedimientos = new List<Procedimiento>();//PROCEDIMIENTOS DEL LIENZO

        List<Funcion> Funciones = new List<Funcion>();//FUNCIONES DEL LIENZO

        List<String> Extends = new List<string>();//EXTENDS DEL LIENZO

        String nombre; //Nombre del Lienzo

        String visibilidad; //Visibilidad del Lienzo
        #endregion

        #region Auxs

        List<Simbolo> auxiliar = new List<Simbolo>(); //LISTA AUXILIAR PARA ALMACENAR SIMBOLOS TEMPORALMENTE

        List<Parametro> paraux = new List<Parametro>();//LISTA AUXILIAR DE PARAMETROS

        List<int> DimensAux = new List<int>();//LISTA AUXILIARES DE DIMENSIONES

        List<List<int>> ValoresAux = new List<List<int>>();//LISTADO AUXILIAR DE VALORES EN DIMENEIONES

        List<int> subValores = new List<int>();

        #endregion

        public Tabla_Simbolos(ParseTreeNode raiz)
        {
            this.raizAST = raiz;
        }

        public List<Simbolo> getTable()//OBTENER LA TABLA ACTUAL
        {
            return this.Tabla;
        }

        public List<Procedimiento> getProcedimientos()//OBTENER LOS PROCEDIMIENTOS ENCONTRADOS
        {
            return this.procedimientos;
        }

        public List<Funcion> getFunciones()//OBTENGO LAS FUNCIONES
        {
            return this.Funciones;
        }

        public List<String> getExtends()//OBTENGO LOS EXTIENDE DEL LIENZO
        {
            return this.Extends;
        }

        public List<Variable> getVariables()
        {
            return this.variables;
        }

        public String getNombre()
        {
            return this.nombre;
        }

        public String getVisibilidad()
        {
            return this.visibilidad;
        }

        public void generarme_tabla()//METODO PUBLICO PARA ACCEDER AL ARBOL 
        {
            generarTabla(this.raizAST);
        }

        private object generarTabla(ParseTreeNode raiz)
        {
            string Inicio = raiz.ToString();
            ParseTreeNode[] hijos = null;
            if (raiz.ChildNodes.Count > 0)
            {
                hijos = raiz.ChildNodes.ToArray();
            }
            switch (Inicio)
            {
                case "INICIO"://CUANDO ME ENCUENTRO EN LA PRODUCCIÓN DEL INICIO
                    {
                        if (raiz.ChildNodes.Count() == 5)
                        {

                            string visi = generarTabla(hijos[0]).ToString();
                            visi = visi.Replace(" (Keyword)", "");
                            this.visibilidad = visi;
                            string tipo = hijos[1].ToString().Replace(" (Keyword)", "");
                            string nombre = hijos[2].ToString().Replace(" (identificador)", "");
                            this.nombre = nombre;
                            ambitos.Push(nombre);
                            //EXTIENDES
                            generarTabla(hijos[3]);
                            Simbolo nuevo = new Simbolo(nombre, tipo, visi, "No Aplica", ambitos.Peek() , false);
                            Tabla.Add(nuevo);
                            //ME MUEVO AL CUERPO DEL LIENZO
                            generarTabla(hijos[4]);
                        }
                        break; 
                    }
                case "EXTIENDE":
                    {
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            generarTabla(hijos[1]);
                        }
                        break;
                    }
                case "LISTADOEX":
                    {
                        if(raiz.ChildNodes.Count() == 3)//::= LISTADOEX com LISTADOEX
                        {
                            generarTabla(hijos[0]);
                            generarTabla(hijos[1]);
                        }
                        if(raiz.ChildNodes.Count() == 1)//::= identificador
                        {
                            String ext = hijos[0].ToString().Replace(" (identificador)", "");
                            this.Extends.Add(ext);
                        }
                        break;
                    }
                case "VISIBILIDAD":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            return hijos[0].ToString();
                        }
                        else
                        {
                            return "privado";
                        }
                        break;
                    }
                case "CUERPOLIENZO":
                    {
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            generarTabla(hijos[0]);
                            generarTabla(hijos[1]);
                        }
                        break;
                    }
                case "PRINCIPAL":
                    {
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            //CAPTURO EL MÉTODO PRINCIPAL
                            string nombre = hijos[0].ToString().Replace(" (Keyword)", "");
                            Simbolo nuevo = new Simbolo(nombre, "Main", "No Aplica", "No Aplica", ambitos.Peek(),false);
                            Procedimiento principal = new Procedimiento(hijos[1], nombre, this.paraux);
                            //METO PROCEDIMIENTOS NUEVOS
                            this.procedimientos.Add(principal);
                            //METO PROCEDMIENTOS NUEVOS
                            ambitos.Push(nombre);
                            Tabla.Add(nuevo);
                            ambitos.Pop();//SALGO DEL AMBITO PRINCIPAL
                        }
                        break;
                    }
                case "OTROS":
                    {
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            generarTabla(hijos[0]);
                            generarTabla(hijos[1]);
                        }
                        break;
                    }
                case "VARGLOBALES":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            string visibi = "No Aplica";
                            //Me retorna si se conserva y llamo al metodo recursivamente para obtener la cadena
                            bool conservar = seConserva(generarTabla(hijos[0]).ToString());
                            //debo de retornar en la producción tipo, el tipo especificado
                            string tipo = generarTabla(hijos[2]).ToString();
                            this.tipo_actual_evaluado = tipo;//TIPO ACTUAL DE VARIABLES EVALUADO
                            //me va a retornar un simbolo en la prducción tipo asignación
                            generarTabla(hijos[3]);//mete los valores a la lista auxiliar
                            string ambito = ambitos.Peek();
                            foreach(Simbolo c in auxiliar)
                            {//aqui podría hacer las comprobaciones semánticas
                                //SIMBOLOS
                                c.visibilidad = visibi;
                                c.conservar = conservar;
                                c.tipo = tipo;
                                c.ambito = ambito;
                                Tabla.Add(c);
                                //VARIABLES:
                                Variable var = new Variable(c.nombre, c.valor, c.tipo, c.ambito, c.conservar, true);
                                variables.Add(var);                        
                            }
                            //LIBERO LA LISTA
                            auxiliar.Clear();
                            tipo_actual_evaluado = null;//se reestablece el tipo global evaluado

                        }
                        break;
                    }
                case "VARLOCALES":
                    {   if(raiz.ChildNodes.Count() == 4)
                        {
                            String visibi = "No Aplica";
                            bool conservar = seConserva(generarTabla(hijos[0]).ToString());
                            String tipo = generarTabla(hijos[2]).ToString();
                            this.tipo_actual_evaluado = tipo;
                            //AQUI ME DIRIJO A TIPO ASIGNACIÓN:
                            generarTabla(hijos[3]);
                            foreach(Simbolo c in auxiliar)
                            {
                                c.visibilidad = visibi;
                                c.conservar = conservar;
                                c.tipo = tipo;
                                c.ambito = ambitos.Peek().ToString();
                                Tabla.Add(c);
                                if(c.esArreglo == true)
                                {
                                    List<List<int>> nueva = new List<List<int>>();
                                    foreach(List<int> l in this.ValoresAux)
                                    {
                                        nueva.Add(l);
                                    }
                                    Variable vari = new Variable(c.nombre, nueva, c.tipo, c.ambito, c.conservar, false, true);
                                    variables.Add(vari);
                                }
                                else
                                {
                                    Variable va = new Variable(c.nombre, c.valor, c.tipo, c.ambito, c.conservar, false);
                                    variables.Add(va);
                                }
                                //AQUI EVALUARÉ SI ES UN ARREGLO PARA PROCEDER
                            }
                            auxiliar.Clear();
                            tipo_actual_evaluado = null;
                        }
                        break;
                    }
                case "PRO_FUNC":
                    {
                        if (raiz.ChildNodes.Count() == 3) //PRO_FUNC ::= CONSERVAR VISIBILIDAD + PRO_FUNCP
                        {
                            Boolean conservar = seConserva(hijos[0].ToString());//VIENDO SI SE CONSERVA
                            String visibilidad = generarTabla(hijos[1]).ToString();//OBTENIENDO VISIVILIDIAD
                            Simbolo simbol = (Simbolo)generarTabla(hijos[2]);//SIMBOLO QUE DETERMINA SI ES FUNCION O PROCEDIMIENTO
                            if(simbol != null)
                            {
                                simbol.conservar = conservar;
                                simbol.visibilidad = visibilidad;
                                this.Tabla.Add(simbol);
                            }
                            //debo de capturar los parametros que tienen
                            this.paraux.Clear();
                            ambitos.Pop();
                        }
                        break;
                    }
                case "PRO_FUNCP":
                    {
                        if (raiz.ChildNodes.Count() == 1) //::= PROCEDIMIENTOS
                        {
                            return generarTabla(hijos[0]);
                        }
                        if(raiz.ChildNodes.Count() == 2)// ::= TIPO  FUNCIONES
                        {
                            String tipo = generarTabla(hijos[0]).ToString();
                            Simbolo simbol = (Simbolo)generarTabla(hijos[1]);
                            ParseTreeNode []subhijos = hijos[1].ChildNodes.ToArray();
                            List<Parametro> parameters = new List<Parametro>();
                            Funcion fun = new Funcion(subhijos[3], simbol.nombre, tipo, addParametersToList(parameters));
                            this.Funciones.Add(fun);
                            return simbol;
                        }
                        break;
                    }
                case "PROCEDIMIENTOS":
                    {
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            //aqui capturo procedimientos
                            string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                            String tipo = "Procedimiento";
                            if(nombre == "Principal")
                            {
                                tipo = "Main";
                            }
                            
                            //PARAMETROS
                            generarTabla(hijos[1]);
                            Simbolo simbolo = new Simbolo(nombre, tipo, "", "No Aplica", ambitos.Peek(), false);
                            ambitos.Push(nombre);
                            List<Parametro> parameters = new List<Parametro>();
                            Procedimiento pro = new Procedimiento(hijos[2], nombre, addParametersToList(parameters));
                            procedimientos.Add(pro);
                            //AQUI FALTAN SENTENCIAS!!!
                            generarTabla(hijos[2]);
                            return simbolo;

                        }
                        break;
                    }
                case "FUNCIONES":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            int dim = 0;
                            dim = (int)generarTabla(hijos[0]);//obtener la dimensión si es que tiene sino tendra 0
                            string nombre = hijos[1].ToString().Replace(" (identificador)", "");
                            //PARAMETROS
                            generarTabla(hijos[2]); 
                            bool esarreglo = false;
                            if(dim > 0)
                            {
                                esarreglo = true;
                            }
                            Simbolo simbolo = new Simbolo(nombre, "Funcion","", "No Aplica", ambitos.Peek(), false, esarreglo, dim);
                            ambitos.Push(nombre);

                            //SENTENCIAS
                            return simbolo;
                        }
                        break;
                    }
                case "DIM":
                    {
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            int retorno = 1;
                            retorno = retorno + (int)generarTabla(hijos[2]);
                            return retorno;
                        }
                        else
                        {
                            return 0;
                        }
                        break;
                    }
                case "PARAMETROS":
                    {
                        if(raiz.ChildNodes.Count() == 3)//:: PARAMETROS , PARAMETROS
                        {
                            generarTabla(hijos[0]);
                            generarTabla(hijos[2]);
                        }
                        if(raiz.ChildNodes.Count() == 2)//TIPO identificador
                        {
                            String tipo = generarTabla(hijos[0]).ToString();
                            String nombre = hijos[1].ToString().Replace(" (identificador)", "");
                            Parametro parametro = new Parametro(nombre, tipo);
                            this.paraux.Add(parametro);
                            
                        }
                        break;
                    }
                case "CONSERVAR":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            return hijos[0].ToString();
                        }
                        else
                        {
                            return "";
                        }
                        //break;
                    }
                case "DIMENSIONES":
                    {
                        if (raiz.ChildNodes.Count() == 4)//::= opb EXPR clb  DIMENSIONES
                        {
                            Expresion exp = new Expresion("entero", this.variables, this.ambitos.Peek());
                            Elemento ele = (Elemento)exp.recorre_expresion(hijos[1]);
                            if(ele != null)
                            {
                                this.DimensAux.Add(Convert.ToInt32(ele.valor));
                            }
                            else
                            {
                                //ERRORES SEMANTICOS
                            }
                        }
                        if(raiz.ChildNodes.Count() == 3)//::= opb EXPR clb
                        {
                            Expresion exp = new Expresion("entero", this.variables, this.ambitos.Peek());
                            Elemento ele = (Elemento)exp.recorre_expresion(hijos[1]);
                            if(ele != null)
                            {
                                this.DimensAux.Add(Convert.ToInt32(ele.valor));
                            }
                            else
                            {
                                //errores semanticos encontrados
                            }
                        }
                        break;
                    }
                case "TIPO":
                    {
                        string tipo = hijos[0].ToString().Replace(" (Keyword)","");
                        return tipo;
                        //break;
                    }
                case "TIPOASIGNACION":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            //ME MUEVO A ASIGNACIÓN
                            generarTabla(hijos[0]);
                        }
                        if(raiz.ChildNodes.Count() == 4)//::= arreglo ASIGNACIONARR DIMENSIONES LLENADOARR
                        {
                            //ME MUEVO A ASIGNACIÓN DE ARREGLO
                            generarTabla(hijos[1]);//OBTENGO SIMBOLOS AUXILIARES
                            generarTabla(hijos[2]);//OBTENGO LAS DIMENSIONES DEL ARREGLO
                            generarTabla(hijos[3]);//LLENADO DEL ARREGLO CON SUS VALORES
                        }
                        break;
                    }
                case "LLENADOARR":
                    {
                        if (raiz.ChildNodes.Count() == 3)//::= opl LISTADOARR cll
                        {
                            generarTabla(hijos[1]);
                            if (this.ValoresAux.Count() == 0)
                            {
                                List<int> v = new List<int>();
                                foreach(int x in this.subValores)
                                {
                                    v.Add(x);
                                }
                                this.ValoresAux.Add(v);
                            }
                        }
                        break;
                    }
                case "LISTADOARR":
                    {
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            if (hijos[1].ToString().Contains(","))//::= LISTADOARR com LISTADOARR
                            {
                                generarTabla(hijos[0]);
                                generarTabla(hijos[2]);
                            }
                            else//::= opl LISTADOARR cll
                            {
                                List<int> nueva = new List<int>();//CREO UNA NUEVA LISTA
                                foreach(int x in this.subValores)//RECORRO LOS SUBVALORES Y LOS METO A LA NUEVA
                                {
                                    nueva.Add(x);
                                }
                                this.subValores.Clear();
                                this.ValoresAux.Add(nueva);//METO LA LISTA DE SUBVALORES A LA LISTA DE LISTAS
                            }
                        }
                        if (raiz.ChildNodes.Count() == 1)//::EXPR
                        {
                            Expresion ex = new Expresion("entero", this.variables, this.ambitos.Peek());
                            Elemento el = (Elemento)ex.recorre_expresion(hijos[0]);
                            if (el != null)
                            {
                                this.subValores.Add(Convert.ToInt32(el.valor));
                            }
                            else
                            {
                                //ERRORES SEMANTICOS
                            }
                        }
                        break;
                    }
                case "ASIGNACIONARR":
                    {
                        if (raiz.ChildNodes.Count() == 3)//::= ASIGNACIONARR com ASIGNACIONARR
                        {

                            generarTabla(hijos[0]);
                            generarTabla(hijos[2]);
                        }
                        if(raiz.ChildNodes.Count() == 1)
                        {
                            string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                            Simbolo sim = new Simbolo(nombre, "", "", "", this.ambitos.Peek(), false, true, 0);
                            this.auxiliar.Add(sim);
                        }
                        break;
                    }
                case "ASIGNACION":
                    {
                        if (raiz.ChildNodes.Count() == 1)//ASIGNAICIÓN SOLO PRODUCE UN IDENTIFICADOR
                        {
                            Simbolo aux = new Simbolo();
                            aux.nombre = hijos[0].ToString().Replace(" (identificador)","");
                            aux.valor = null;
                            auxiliar.Add(aux);//AÑADO A LISTA AUXILIAR
                        }
                        if(raiz.ChildNodes.Count() == 2)//CUANDO VIENE UNA ASIGNACIÓN DE UNA VARIABLE CON SU VALOR
                        {
                            //PENDIENTE DE VERIFICAR SU VALOR
                            Simbolo aux = new Simbolo();
                            aux.nombre = hijos[0].ToString().Replace(" (identificador)", "");
                            //evaluo una nueva expresión
                            Expresion expr = new Expresion(this.tipo_actual_evaluado, this.variables, ambitos.Peek());
                            Elemento ele = (Elemento)expr.recorre_expresion(hijos[1]);
                            if(ele != null)
                            {
                                aux.valor = ele.valor;//RECORRO LA EXPRESIÓN PARA OBTENER EL VALOR
                                this.auxiliar.Add(aux);//añado a la lista auxiliar
                            }
                            else
                            {
                                //SEMANTICOS
                                List<ErrorEnAnalisis> errores = expr.getErroresSemanticos();
                                foreach(ErrorEnAnalisis er in errores)
                                {
                                    MessageBox.Show(er.getError() + " | Linea: " + er.getLinea() + " | Columna: " + er.getColumna());
                                }
                            }
                        }
                        if(raiz.ChildNodes.Count() == 3)//ASIGNACIÓN PRODUCE UN LISTADO DE IDS
                        {
                            generarTabla(hijos[0]);//ME VOY A ASIGNACIÓN DE UN HIJO
                            generarTabla(hijos[2]);//ASIGNACIÓN RECURSIVA
                        }
                        break;
                    }
                case "SENTENCIAS":
                    {
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            if (hijos[0].ToString().Contains("VARLOCALES"))
                            {
                                generarTabla(hijos[0]);
                                generarTabla(hijos[1]);
                            }
                        }
                        break;
                    } 
            }
            return "";
        }


        private bool seConserva(string cadena)
        {
            if(cadena == "Conservar")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private List<Parametro> addParametersToList(List<Parametro> parameter)
        {
            foreach(Parametro p in this.paraux)
            {
                parameter.Add(p);
            }
            return parameter;
        }

    }
}
