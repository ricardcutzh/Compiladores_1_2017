using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lienzo2D.Analizador;

namespace Lienzo2D.Clases
{
    class Ejecucion
    {
        #region Objetos de Ejecucion
        List<Lienzo> LienzosCompilados = new List<Lienzo>();//LIENZOS QUE SE EJECUTARON
        List<Simbolo> TablaGeneral = new List<Simbolo>();//TABLA DE SIMBOLOS
        List<ErrorEnAnalisis> errores = new List<ErrorEnAnalisis>();

        String nombreFuncion;
        #endregion

        #region Variables Auxiliars
        List<int> indices = new List<int>();

        Stack<String> ambitosEje = new Stack<string>();

        List<Parametro> paraAux = new List<Parametro>();

        int contador = 0;

        int line;

        int column;

        bool retorna = false;

        string tipoFun;

        Lienzo LienzoEjecutando;//AUXILIAR DE LIENZO
        #endregion

        #region Gets
        public List<Simbolo> getTablaNueva()
        {
            return this.TablaGeneral;
        }
        public List<ErrorEnAnalisis> getErrores()
        {
            return this.errores;
        }
        #endregion

        #region Construtores
        public Ejecucion( List<Lienzo> LienzosCompilados, List<Simbolo> TablaDeSimbolos)
        {
            this.LienzosCompilados = LienzosCompilados;
            this.TablaGeneral = TablaDeSimbolos;
        }

        public Ejecucion( List<Lienzo> LienzosCompilados, List<Simbolo> TablaDeSimbolos, String nombreFuncion)
        {
            this.LienzosCompilados = LienzosCompilados;
            this.TablaGeneral = TablaDeSimbolos;
            this.nombreFuncion = nombreFuncion;
        }
        #endregion

        #region Metodos Auxiliares

        public void IniciarEjecucion()
        {
            try
            {
                Lienzo Padre = this.LienzosCompilados.ElementAt(0);
                Procedimiento main = MetodoPrincipal(Padre);
                this.LienzoEjecutando = Padre;
                this.ambitosEje.Push(main.Nombre);
                //Padre.reporteDeArreglos();
                Ejecutar(main.Sentencias);
                //Padre.reporteVariables();
                Padre.reporteDeArreglos();
            }
            catch
            {
                Console.WriteLine("error al iniciar la ejecución");
            }
        }

        public Object ejecutarFuncion()
        {
            Funcion funEjecutar = null;
            foreach(Lienzo l in this.LienzosCompilados)
            {
                foreach(Funcion f in l.Funciones)
                {
                    if(f.Nombre == this.nombreFuncion)
                    {
                        funEjecutar = f;
                        break;
                    }
                }
            }
            this.retorna = false;
            this.tipoFun = funEjecutar.Tipo;
            Elemento n = (Elemento)EjecucionFuncion(funEjecutar.Sentencias);
            return n;
        }

        public Funcion buscarFuncion(string nombre)
        {
            Funcion f = null;
            foreach(Lienzo l in LienzosCompilados)
            {
                foreach(Funcion j in l.Funciones)
                {
                    if(j.Nombre == nombre)
                    {
                        f = j;
                        break;
                    }
                }
            }
            return f;
        }

        private Procedimiento MetodoPrincipal(Lienzo padre)
        {
            List<Procedimiento> procedimientos = padre.Procedimientos;
            Procedimiento principal = null;
            foreach(Procedimiento p in procedimientos)
            {
                if(p.Nombre == "Principal")
                {
                    principal = p;
                    //CONSOLA MENSAJE
                    Console.WriteLine("Encontre Principal");
                    //FIN MENSAJE CONSOLA
                    break;
                }
            }
            return principal;
        }

        private void updateTablaDeSimbolos(string nombre, string tipo, string ambito, string valor)
        {
            foreach(Simbolo s in this.TablaGeneral)
            {
                if(s.nombre == nombre && s.tipo == tipo && s.ambito == ambito)
                {
                    s.valor = valor;
                }
            }
        }

        private bool buscaVariableYAsignaValor(string nombre, string nuevoavalor, string ambito, string tipo)
        {
            bool bandera = false;
            foreach(Variable b in this.LienzoEjecutando.Variables)
            {
                if (!b.esArreglo)
                {
                    if(b.nombre == nombre)
                    {
                        if(b.ambito == ambito)
                        {
                            if(b.tipo == tipo)
                            {
                                bandera = true;
                                b.valor = nuevoavalor;
                                updateTablaDeSimbolos(b.nombre, b.tipo, b.ambito, b.valor);
                            }
                            else
                            {
                                ErrorEnAnalisis erro = new ErrorEnAnalisis("No se puede Asignar un tipo: " + tipo + " a una variable de tipo: " + b.tipo, "Error Semantico", this.line, this.column);
                                this.errores.Add(erro);
                            }
                        }
                    }
                }
            }
            //AQUI PUEDO BUSCAR DENTRO DE LOS PARAMETROS
            foreach(Parametro p in this.paraAux)
            {
                if(nombre == p.nombre)
                {
                    p.valor = nuevoavalor;
                    bandera = true;
                }
            }
            //AQUI TERMINO DE BUSCAR EN PARAMETROS
            if (bandera == true)
            {
                return bandera;
            }
            else
            {
                foreach(Variable n in this.LienzoEjecutando.Variables)
                {
                    if (!n.esArreglo)
                    {
                        if(n.nombre == nombre && n.esGlobal)
                        {
                            if(n.tipo == tipo)
                            {
                                n.valor = nuevoavalor;
                                updateTablaDeSimbolos(n.nombre, n.tipo, this.LienzoEjecutando.nombre, nuevoavalor);
                                bandera = true;
                            }
                            else
                            {
                                ErrorEnAnalisis erro = new ErrorEnAnalisis("No se puede Asignar un tipo: " + tipo + " a una variable de tipo: " + n.tipo, "Error Semantico", this.line, this.column);
                                this.errores.Add(erro);
                            }
                        }
                    }
                }
            }
            return bandera;
        }

        private bool BuscaArregloYAsigna(string nombre, string ambito, string nuevovalor, string tipo)
        {
            bool bandera = false;
            foreach(Variable v in this.LienzoEjecutando.Variables)
            {
                if (v.esArreglo)
                {
                    if(v.nombre == nombre && v.ambito == ambito)
                    {
                        if(v.tipo == tipo)
                        {
                            if (this.indices.Count() == 1)
                            {
                                if(this.indices.ElementAt(0)< v.Valores.ElementAt(0).Count())
                                {
                                    v.Valores.ElementAt(0).Insert(this.indices.ElementAt(0), Convert.ToInt32(nuevovalor));
                                    v.Valores.ElementAt(0).RemoveAt(this.indices.ElementAt(0) + 1);
                                    bandera = true;
                                }
                                else
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Indice fuera de los limites del Arreglo: ' " + v.nombre + " '", "Error Semantico", this.line, this.column);
                                    this.errores.Add(error);
                                }
                            }
                        }
                        else
                        {
                            ErrorEnAnalisis erro = new ErrorEnAnalisis("No se puede Asignar un tipo: " + tipo + " a una variable de tipo: " + v.tipo, "Error Semantico", this.line, this.column);
                            this.errores.Add(erro);
                        }
                    }
                }
            }
            if(bandera == true)
            {
                return bandera;
            }
            else
            {
                foreach(Variable v in this.LienzoEjecutando.Variables)
                {
                    if(v.esArreglo && v.esGlobal)
                    {
                        if(v.nombre == nombre)
                        {
                            if(v.tipo == tipo)
                            {
                                v.Valores.ElementAt(0).Insert(this.indices.ElementAt(0), Convert.ToInt32(nuevovalor));
                                bandera = true;
                            }
                            else
                            {
                                ErrorEnAnalisis erro = new ErrorEnAnalisis("No se puede Asignar un tipo: " + tipo + " a una variable de tipo: " + v.tipo, "Error Semantico", this.line, this.column);
                                this.errores.Add(erro);
                            }
                        }
                    }
                }
            }
            return bandera;
        }

        private String getTipoDeVar(string nombre, string ambito)
        {
            string tipo = "";
            bool band = false;
            foreach(Variable h in this.LienzoEjecutando.Variables)
            {
                if (h.nombre == nombre && h.ambito == ambito)
                {
                    tipo = h.tipo;
                    band = true;
                }
            }
            if(band == true)
            {
                return tipo;
            }
            else
            {
                foreach(Variable j in this.LienzoEjecutando.Variables)
                {
                    if(j.nombre == nombre && j.esGlobal)
                    {
                        tipo = j.tipo;
                    }
                }
            }
            return tipo;
        }

        private Variable buscarVar(string id)
        {
            bool bandera = false;
            Variable v = null;
            foreach(Variable k in this.LienzoEjecutando.Variables)
            {
                if(k.nombre == id)
                {
                    bandera = true;
                    v = k;
                }
            }
            if(bandera == false)
            {
                foreach(Parametro p in this.paraAux)
                {
                    if(p.nombre == id)
                    {
                        v = new Variable(id, v.valor, p.tipo, this.ambitosEje.Peek(), false, false);
                    }
                }
            }
            return v;
        }
        #endregion

        #region Ejecucion de Codigo
        private Object Ejecutar(ParseTreeNode raiz)
        {
            string Inicio = raiz.ToString();
            ParseTreeNode[] hijos = null;
            if (raiz.ChildNodes.Count > 0)
            {
                hijos = raiz.ChildNodes.ToArray();
            }
            switch (Inicio)
            {
                case "SENTENCIAS":
                    {
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            //ME MUEVO A LA SENTENCIA QUE TOCARÍA:
                            Ejecutar(hijos[0]);
                            //ME MUEVO RECURSIVAMENTE A SENTENCIAS
                            Ejecutar(hijos[1]);//SENTENCIAS
                        }
                        break;
                    }
                case "VARLOCALES":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            Ejecutar(hijos[3]);
                        }
                        break;
                    }
                case "TIPOASIGNACION":
                    {
                        if(raiz.ChildNodes.Count() == 4)
                        {
                            string nombreAsignar = hijos[1].ToString();

                            Ejecutar(hijos[3]);
                        }
                        break;
                    }
                case "ASIGNACIONARR":
                    {
                        if(raiz.ChildNodes.Count() == 1)
                        {
                            return hijos[0].ToString().Replace(" (identificador)", "");
                        }
                        break;
                    }
                case "LLENADOARR":
                    {
                        if (raiz.ChildNodes.Count == 1)
                        {
                            Ejecutar(hijos[0]);
                        }
                        break;
                    }
                case "RESULTADOFUN":
                    {
                        string nombreDeFuncion = hijos[0].ToString().Replace(" (identificador)", "");
                        Funcion f = buscarFuncion(nombreDeFuncion);
                        this.contador = 0;
                        this.paraAux = f.Parametros;
                        Ejecutar(hijos[1]);
                        //AQUI ME FALTA LA LLAMADA A RESULTADO DE LA FUNCION
                        //AQUI DEBO DE HACER UNA VARIABLE GLOBAL CON LA CUAL PUEDA ALMACENAR TEMPORALMENTE LA VARIABLE
                        //QUE ALMACENO O QUE RETORNA LA FUNCION PARA LUEGO PONERLA EN DONDE CORRESPOONDE SEGUN SU VALOR
                        break;
                    }
                case "LISTADOEXPRE":
                    {
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            Ejecutar(hijos[0]);
                            Ejecutar(hijos[2]);
                        }
                        if(raiz.ChildNodes.Count() == 1)
                        {
                            Elemento ele = null;
                            try
                            {
                                Expresion exp = new Expresion(this.paraAux.ElementAt(this.contador).tipo, this.LienzoEjecutando.Variables, ambitosEje.Peek());
                                ele = (Elemento)exp.recorre_expresion(hijos[0]);
                            }
                            catch
                            {
                                ele = null;
                            }
                            if(ele != null)
                            {
                                try
                                {
                                    this.paraAux.ElementAt(this.contador).valor = ele.valor;
                                }
                                catch
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("Parametros no Concuerdan", "Error Semantico", this.line, this.column);
                                    this.errores.Add(error);
                                }   
                            }
                            else
                            {
                                ErrorEnAnalisis error = new ErrorEnAnalisis("Parametros no Concuerdan", "Error Semantico", this.line, this.column);
                                this.errores.Add(error);
                            }
                            this.contador = this.contador + 1;
                        }
                        break;
                    }
                case "ASIGNAVAR":
                    {
                        if(raiz.ChildNodes.Count()== 3)//::= identificador DIMOPCIONAL EXPR
                        {
                            this.line = hijos[0].Token.Location.Line;
                            this.column = hijos[0].Token.Location.Column;
                            //CAPTURO EL NOMBRE DE LA VARIABLE A ASIGNAR
                            string nombreAsignar = hijos[0].ToString().Replace(" (identificador)", "");
                            string tipoAsingnar = getTipoDeVar(nombreAsignar, this.ambitosEje.Peek());
                            //debo obtener el tipo de variable que es..
                            //ME MUEVO A DIMENSIONES OPCIONALES
                            Ejecutar(hijos[1]);
                            if (tipoAsingnar != "")
                            {
                                Expresion ex = new Expresion(tipoAsingnar, this.LienzoEjecutando.Variables, this.ambitosEje.Peek());
                                Elemento el = (Elemento)ex.recorre_expresion(hijos[2]);
                                if(el != null)
                                {
                                    if (indices.Count() > 0)
                                    {
                                        BuscaArregloYAsigna(nombreAsignar, ambitosEje.Peek(), el.valor, tipoAsingnar);
                                        this.indices.Clear();
                                    }
                                    else
                                    {
                                        buscaVariableYAsignaValor(nombreAsignar, el.valor, ambitosEje.Peek(), tipoAsingnar);
                                    }
                                }
                                else
                                {
                                    List<ErrorEnAnalisis> aux = ex.getErroresSemanticos();
                                    foreach(ErrorEnAnalisis c in aux)
                                    {
                                        this.errores.Add(c);
                                    }
                                }
                            }
                            else
                            {
                                ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe una Variable: ' " + nombreAsignar + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", this.line, this.column);
                                this.errores.Add(error);
                            }
                            
                        }
                        break;
                    }
                case "DIMOPCIONAL":
                    {
                        if(raiz.ChildNodes.Count() == 1)
                        {
                            //ME MUEVO A DIMENSIONES
                            Ejecutar(hijos[0]);
                        }
                        break;
                    }
                case "DIMENSIONES":
                    {
                        if(raiz.ChildNodes.Count() == 4)//::= opb EXPR clb DIMENSIONES
                        {
                            Expresion expr = new Expresion("entero", this.LienzoEjecutando.Variables, this.ambitosEje.Peek());
                            Elemento el = (Elemento)expr.recorre_expresion(hijos[1]);
                            if(el != null)
                            {
                                this.indices.Add(Convert.ToInt32(el.valor));
                            }
                            else
                            {
                                List<ErrorEnAnalisis> auxier = expr.getErroresSemanticos();
                                foreach(ErrorEnAnalisis c in auxier)
                                {
                                    this.errores.Add(c);
                                }
                            }
                            Ejecutar(hijos[3]);
                        }
                        if(raiz.ChildNodes.Count() == 3)//::= opb EXPR clb
                        {
                            Expresion expr = new Expresion("entero", this.LienzoEjecutando.Variables, this.ambitosEje.Peek());
                            Elemento el = (Elemento)expr.recorre_expresion(hijos[1]);
                            if(el != null)
                            {
                                this.indices.Add(Convert.ToInt32(el.valor));
                            }
                            else
                            {
                                List<ErrorEnAnalisis> auxier = expr.getErroresSemanticos();
                                foreach (ErrorEnAnalisis c in auxier)
                                {
                                    this.errores.Add(c);
                                }
                            }
                        }
                        break;
                    }
            }
            return "";
        }

        public Object EjecucionFuncion(ParseTreeNode raiz)
        {
            Variable retorno = null;
            string Inicio = raiz.ToString();
            ParseTreeNode[] hijos = null;
            if (raiz.ChildNodes.Count > 0)
            {
                hijos = raiz.ChildNodes.ToArray();
            }
            switch (Inicio)
            {
                case "SENTENCIASP":
                    {
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            //ME MUEVO A LA SENTENCIA QUE TOCARÍA:
                            Ejecutar(hijos[0]);
                            if (!retorna)
                            {
                                //ME MUEVO RECURSIVAMENTE A SENTENCIAS
                                EjecucionFuncion(hijos[1]);//SENTENCIASP
                            }
                        }
                        break;
                    }
                case "ASIGNAVAR":
                    {
                        if (raiz.ChildNodes.Count() == 3)//::= identificador DIMOPCIONAL EXPR
                        {
                            this.line = hijos[0].Token.Location.Line;
                            this.column = hijos[0].Token.Location.Column;
                            //CAPTURO EL NOMBRE DE LA VARIABLE A ASIGNAR
                            string nombreAsignar = hijos[0].ToString().Replace(" (identificador)", "");
                            string tipoAsingnar = getTipoDeVar(nombreAsignar, this.ambitosEje.Peek());
                            //debo obtener el tipo de variable que es..
                            //ME MUEVO A DIMENSIONES OPCIONALES
                            Ejecutar(hijos[1]);
                            if (tipoAsingnar != "")
                            {
                                Expresion ex = new Expresion(tipoAsingnar, this.LienzoEjecutando.Variables, this.ambitosEje.Peek());
                                Elemento el = (Elemento)ex.recorre_expresion(hijos[2]);
                                if (el != null)
                                {
                                    if (indices.Count() > 0)
                                    {
                                        BuscaArregloYAsigna(nombreAsignar, ambitosEje.Peek(), el.valor, tipoAsingnar);
                                        this.indices.Clear();
                                    }
                                    else
                                    {
                                        buscaVariableYAsignaValor(nombreAsignar, el.valor, ambitosEje.Peek(), tipoAsingnar);
                                    }
                                }
                                else
                                {
                                    List<ErrorEnAnalisis> aux = ex.getErroresSemanticos();
                                    foreach (ErrorEnAnalisis c in aux)
                                    {
                                        this.errores.Add(c);
                                    }
                                }
                            }
                            else
                            {
                                ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe una Variable: ' " + nombreAsignar + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", this.line, this.column);
                                this.errores.Add(error);
                            }

                        }
                        break;
                    }
                case "DIMOPCIONAL":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            //ME MUEVO A DIMENSIONES
                            EjecucionFuncion(hijos[0]);
                        }
                        break;
                    }
                case "DIMENSIONES":
                    {
                        if (raiz.ChildNodes.Count() == 4)//::= opb EXPR clb DIMENSIONES
                        {
                            Expresion expr = new Expresion("entero", this.LienzoEjecutando.Variables, this.ambitosEje.Peek());
                            Elemento el = (Elemento)expr.recorre_expresion(hijos[1]);
                            if (el != null)
                            {
                                this.indices.Add(Convert.ToInt32(el.valor));
                            }
                            else
                            {
                                List<ErrorEnAnalisis> auxier = expr.getErroresSemanticos();
                                foreach (ErrorEnAnalisis c in auxier)
                                {
                                    this.errores.Add(c);
                                }
                            }
                            EjecucionFuncion(hijos[3]);
                        }
                        if (raiz.ChildNodes.Count() == 3)//::= opb EXPR clb
                        {
                            Expresion expr = new Expresion("entero", this.LienzoEjecutando.Variables, this.ambitosEje.Peek());
                            Elemento el = (Elemento)expr.recorre_expresion(hijos[1]);
                            if (el != null)
                            {
                                this.indices.Add(Convert.ToInt32(el.valor));
                            }
                            else
                            {
                                List<ErrorEnAnalisis> auxier = expr.getErroresSemanticos();
                                foreach (ErrorEnAnalisis c in auxier)
                                {
                                    this.errores.Add(c);
                                }
                            }
                        }
                        break;
                    }
                case "RETORNAR":
                    {
                        if (raiz.ChildNodes.Count()==2)
                        {
                            retorna = true;
                            Variable r = buscarVar(hijos[1].ToString().Replace(" (identificador)", ""));
                            retorno = r;
                        }
                        break;
                    }
            }
            return retorno;
        }
        #endregion
    }
}
