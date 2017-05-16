using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Lienzo2D.Analizador;
using System.Drawing;

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

        List<Parametro> parametrosAuxiliares = new List<Parametro>();
        String nombreProcedimiento;

        Variable varauxiliar;
        String nombreAsignar;

        string tipo_evaluado;

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
        public List<Variable> getVariablesDespuesDeEjecucion()
        {
            return this.LienzoEjecutando.Variables;
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
                ejecutaFunciones(main.Sentencias);
                IniciaObteniendoParametros();
                
                //Padre.reporteDeArreglos();
                Ejecutar(main.Sentencias);
                Padre.reporteVariables();
                //Padre.reporteDeArreglos();
            }
            catch
            {
                Console.WriteLine("error al iniciar la ejecución");
            }
        }


        private void IniciaObteniendoParametros()
        {
            foreach(Lienzo l in this.LienzosCompilados)
            {
                foreach(Procedimiento p in l.Procedimientos)
                {
                    obtenerParametrosDeFunciones(p.Sentencias);
                }
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

        public Procedimiento buscarProcedimiento(string nombre)
        {
            Procedimiento n = null;
            foreach(Lienzo l in this.LienzosCompilados)
            {
                foreach(Procedimiento p in l.Procedimientos)
                {
                    if(p.Nombre == nombre)
                    {
                        n = p;
                    }
                }
            }
            return n;
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

        private string buscaValorDeVar(string nombre, string ambito, string tipo)
        {
            string valor = "";
            foreach(Variable v in this.LienzoEjecutando.Variables)
            {
                if(v.nombre == nombre && v.tipo == tipo && v.ambito == ambito)
                {
                    valor = v.valor;
                    break;
                }
            }
            return valor;
        }

        private void asignarValorDeFuncion(string nombre)
        {
            foreach(Variable k in this.LienzoEjecutando.Variables)
            {
                if(k.nombre == nombre)
                {
                    k.valor = this.varauxiliar.valor;
                    k.Valores = this.varauxiliar.Valores;
                }
            }
        }
        #endregion

        #region Ejecucion de Codigo

        private Object ejecutaFunciones(ParseTreeNode raiz)
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
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            //ME MUEVO A LA SENTENCIA QUE TOCARÍA:
                            ejecutaFunciones(hijos[0]);
                            //ME MUEVO RECURSIVAMENTE A SENTENCIAS
                            ejecutaFunciones(hijos[1]);//SENTENCIAS
                        }
                        break;
                    }
                case "VARLOCALES":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            ejecutaFunciones(hijos[3]);
                        }
                        break;
                    }
                case "TIPOASIGNACION":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            string nombreAsignar = ejecutaFunciones(hijos[1]).ToString();
                            this.nombreAsignar = nombreAsignar;
                            ejecutaFunciones(hijos[3]);
                        }
                        break;
                    }
                case "ASIGNACIONARR":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            return hijos[0].ToString().Replace(" (identificador)", "");
                        }
                        break;
                    }
                case "LLENADOARR":
                    {
                        if (raiz.ChildNodes.Count == 1)
                        {
                            ejecutaFunciones(hijos[0]);
                        }
                        break;
                    }
                case "RESULTADOFUN":
                    {
                        string nombreDeFuncion = hijos[0].ToString().Replace(" (identificador)", "");
                        Funcion f = buscarFuncion(nombreDeFuncion);
                        this.contador = 0;
                        this.paraAux = f.Parametros;
                        ejecutaFunciones(hijos[1]);
                        //AQUI ME FALTA LA LLAMADA A RESULTADO DE LA FUNCION
                        //AQUI DEBO DE HACER UNA VARIABLE GLOBAL CON LA CUAL PUEDA ALMACENAR TEMPORALMENTE LA VARIABLE
                        //QUE ALMACENO O QUE RETORNA LA FUNCION PARA LUEGO PONERLA EN DONDE CORRESPOONDE SEGUN SU VALOR
                        //this.varauxiliar;
                        ambitosEje.Push(nombreDeFuncion);
                        EjecucionFuncion(f.Sentencias);
                        ambitosEje.Pop();
                        if (this.varauxiliar != null)
                        {
                            asignarValorDeFuncion(this.nombreAsignar);
                        }
                        break;
                    }
                case "LISTADOEXPRE":
                    {
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            ejecutaFunciones(hijos[0]);
                            ejecutaFunciones(hijos[2]);
                        }
                        if (raiz.ChildNodes.Count() == 1)
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
                            if (ele != null)
                            {
                                try
                                {
                                    this.paraAux.ElementAt(this.contador).valor = ele.valor;
                                    Variable nueva = new Variable(this.paraAux.ElementAt(this.contador).nombre, ele.valor, this.paraAux.ElementAt(this.contador).tipo, this.ambitosEje.Peek(), false, false);
                                    this.LienzoEjecutando.Variables.Add(nueva);
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
                            ejecutaFunciones(hijos[3]);
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
            }
            return "";
        }

        private Object obtenerParametrosDeFunciones(ParseTreeNode raiz)
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
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            obtenerParametrosDeFunciones(hijos[0]);
                            obtenerParametrosDeFunciones(hijos[1]);
                        }
                        break;
                    }
                case "FUN_PRO":
                    {
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            this.contador = 0;
                            String nombrePro = hijos[0].ToString().Replace(" (identificador)","");
                            this.nombreProcedimiento = nombrePro;
                            Procedimiento p = buscarProcedimiento(nombrePro);
                            if(p != null)
                            {
                                this.parametrosAuxiliares = p.parametros;
                            }
                            obtenerParametrosDeFunciones(hijos[1]);
                        }
                        break;
                    }
                case "LISTADOEXPRE":
                    {
                        if(raiz.ChildNodes.Count() == 3)//:= LISTADOEXPRE com LISTADOEXPRE
                        {
                            obtenerParametrosDeFunciones(hijos[0]);
                            obtenerParametrosDeFunciones(hijos[2]);
                        }
                        if(raiz.ChildNodes.Count() == 1)
                        {
                            Elemento ele = null;
                            try
                            {
                                Expresion exp = new Expresion(this.parametrosAuxiliares.ElementAt(this.contador).tipo, this.LienzoEjecutando.Variables, ambitosEje.Peek());
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
                                    this.parametrosAuxiliares.ElementAt(this.contador).valor = ele.valor;
                                    Variable nueva = new Variable(this.parametrosAuxiliares.ElementAt(this.contador).nombre, ele.valor, this.parametrosAuxiliares.ElementAt(this.contador).tipo, this.nombreProcedimiento, false, false);
                                    Simbolo simbolo = new Simbolo(this.parametrosAuxiliares.ElementAt(this.contador).nombre, this.parametrosAuxiliares.ElementAt(this.contador).tipo, "No Aplica", "", this.nombreProcedimiento, false);
                                    this.TablaGeneral.Add(simbolo);
                                    this.LienzoEjecutando.Variables.Add(nueva);
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
                case "SENTENCIASP":
                    {
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            obtenerParametrosDeFunciones(hijos[0]);
                            obtenerParametrosDeFunciones(hijos[1]);
                        }
                        break;
                    }
            }
            return "";
        }

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
                            this.tipo_evaluado = Ejecutar(hijos[2]).ToString();
                            Ejecutar(hijos[3]);
                            this.tipo_evaluado = null;
                        }
                        break;
                    }
                case "TIPO":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            string tipo = hijos[0].ToString().Replace(" (Keyword)", "");
                            return tipo;
                        }
                        break;
                    }
                case "TIPOASIGNACION":
                    {
                        if(raiz.ChildNodes.Count() == 4)
                        {
                            string nombreAsignar = Ejecutar(hijos[1]).ToString();
                            this.nombreAsignar = nombreAsignar;
                            Ejecutar(hijos[3]);
                        }
                        if(raiz.ChildNodes.Count() == 1)
                        {
                            Ejecutar(hijos[0]);
                        }
                        break;
                    }
                case "ASIGNACION":
                    {
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            //obtengo el nombre de la variable ya asignada
                            string nombreDeVar = hijos[0].ToString().Replace(" (identificador)", "");
                            string ambito = ambitosEje.Peek();//ambito en el que estoy
                            Expresion exp = new Expresion(this.tipo_evaluado, this.LienzoEjecutando.Variables, ambito);
                            Elemento el = (Elemento)exp.recorre_expresion(hijos[1]);
                            if(el != null)
                            {
                                buscaVariableYAsignaValor(nombreDeVar, el.valor, ambito, this.tipo_evaluado);
                            }
                            else
                            {
                                
                                foreach(ErrorEnAnalisis f in exp.getErroresSemanticos())
                                {
                                    this.errores.Add(f);
                                }
                            }

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
                        //this.varauxiliar;
                        if(this.varauxiliar != null)
                        {
                            asignarValorDeFuncion(this.nombreAsignar);
                        }
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
                                    Variable nueva = new Variable(this.paraAux.ElementAt(this.contador).nombre, ele.valor, this.paraAux.ElementAt(this.contador).tipo, this.ambitosEje.Peek(), false, false);
                                    this.LienzoEjecutando.Variables.Add(nueva);
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
                case "FUN_PRO":
                    {
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                            Procedimiento p = (Procedimiento)buscarProcedimiento(nombre);
                            if (p != null)
                            {
                                ambitosEje.Push(nombre);
                                Console.Write("Ejecutando: " + nombre+" \n");
                                ejecutaFunciones(p.Sentencias);
                                Ejecutar(p.Sentencias);
                                ambitosEje.Pop();
                            }
                            else
                            {
                                Funcion f = (Funcion)buscarFuncion(nombre);
                                ambitosEje.Push(nombre);
                                ejecutaFunciones(f.Sentencias);
                                Ejecutar(p.Sentencias);
                                ambitosEje.Pop();
                            }
                        }
                        break;
                    }
                case "SENTE_AU":
                    {
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            if (hijos[2].ToString().Contains("EXPR"))//::= identificador mas EXPR
                            {
                                string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                                string tipo = getTipoDeVar(nombre, ambitosEje.Peek());
                                string currentvalor = buscaValorDeVar(nombre, ambitosEje.Peek(), tipo);
                                Expresion exp = new Expresion(tipo, this.LienzoEjecutando.Variables, ambitosEje.Peek());
                                Elemento el = (Elemento)exp.recorre_expresion(hijos[2]);
                                if(el != null)
                                {
                                    if (currentvalor != "")
                                    {
                                        int current = Convert.ToInt32(currentvalor);
                                        int addnew = Convert.ToInt32(el.valor);
                                        int nuevoval = current + addnew;
                                        if (!buscaVariableYAsignaValor(nombre, nuevoval.ToString(), ambitosEje.Peek(), tipo))
                                        {
                                            ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                            this.errores.Add(error);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach(ErrorEnAnalisis s in exp.getErroresSemanticos())
                                    {
                                        this.errores.Add(s);
                                    }
                                }
                            }
                            else//:identificador mas mas
                            {
                                string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                                string tipo = getTipoDeVar(nombre, ambitosEje.Peek());
                                string currentValue = buscaValorDeVar(nombre, ambitosEje.Peek(), tipo);
                                if (currentValue!="")
                                {
                                    if(tipo == "entero")
                                    {
                                        int current = Convert.ToInt32(currentValue);
                                        int nuevoval = current + 1;
                                        if(!buscaVariableYAsignaValor(nombre, nuevoval.ToString(), ambitosEje.Peek(), tipo))
                                        {
                                            ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                            this.errores.Add(error);
                                        }
                                    }
                                    else
                                    {
                                        ErrorEnAnalisis error = new ErrorEnAnalisis("No se puede realizar al aumento a una variable de tipo: "+tipo, "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                        this.errores.Add(error);
                                    }
                                }
                                else
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este ambito: " + ambitosEje.Peek(),"Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                    this.errores.Add(error);
                                }
                            }
                        }
                        break;
                    }
                case "SENTE_DEC":
                    {
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            if (hijos[2].ToString().Contains("EXPR"))
                            {
                                string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                                string tipo = getTipoDeVar(nombre, ambitosEje.Peek());
                                string currentvalor = buscaValorDeVar(nombre, ambitosEje.Peek(), tipo);
                                Expresion exp = new Expresion(tipo, this.LienzoEjecutando.Variables, ambitosEje.Peek());
                                Elemento el = (Elemento)exp.recorre_expresion(hijos[2]);
                                if (el != null)
                                {
                                    if (currentvalor != "")
                                    {
                                        int current = Convert.ToInt32(currentvalor);
                                        int addnew = Convert.ToInt32(el.valor);
                                        int nuevoval = current - addnew;
                                        if (!buscaVariableYAsignaValor(nombre, nuevoval.ToString(), ambitosEje.Peek(), tipo))
                                        {
                                            ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                            this.errores.Add(error);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (ErrorEnAnalisis s in exp.getErroresSemanticos())
                                    {
                                        this.errores.Add(s);
                                    }
                                }
                            }
                            else//::= identificador menos menos
                            {
                                string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                                string tipo = getTipoDeVar(nombre, ambitosEje.Peek());
                                string currentValue = buscaValorDeVar(nombre, ambitosEje.Peek(), tipo);
                                if (currentValue != "")
                                {
                                    if (tipo == "entero")
                                    {
                                        int current = Convert.ToInt32(currentValue);
                                        int nuevoval = current - 1;
                                        if (!buscaVariableYAsignaValor(nombre, nuevoval.ToString(), ambitosEje.Peek(), tipo))
                                        {
                                            ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                            this.errores.Add(error);
                                        }
                                    }
                                    else
                                    {
                                        ErrorEnAnalisis error = new ErrorEnAnalisis("No se puede realizar al aumento a una variable de tipo: " + tipo, "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                        this.errores.Add(error);
                                    }
                                }
                                else
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                    this.errores.Add(error);
                                }
                            }
                        }
                        break;
                    }
                case "SENTENCIA_SI":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            Logica log = new Logica(this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            bool con = (Boolean)log.EvaluaLogica(hijos[1]);
                            if(con != null)
                            {
                                if (con)
                                {
                                    Ejecutar(hijos[2]);
                                }
                                else
                                {
                                    Ejecutar(hijos[3]);
                                }
                            }
                        }
                        break;
                    }
                case "SINO":
                    {
                        if (raiz.ChildNodes.Count() == 2)
                        {
                            Ejecutar(hijos[1]);
                        }
                        break;
                    }
                case "SENTENCIA_MIENTRAS":
                    {
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            EjecutaCicloMientras(hijos[2], hijos[1]);
                        }
                        break;
                    }
                case "SENTENCIA_HACER":
                    {
                        if(raiz.ChildNodes.Count() == 4)
                        {
                            EjecutaCicloHacerMientras(hijos[1], hijos[3]);
                        }
                        break;
                    }
                case "SENTENCIA_PARA":
                    {
                        if(raiz.ChildNodes.Count() == 5)
                        {
                            Ejecutar(hijos[1]);
                            EjecutaCicloPara(hijos[4], hijos[2], hijos[3]);
                            int rem = this.LienzoEjecutando.Variables.Count() - 1;
                            this.LienzoEjecutando.Variables.RemoveAt(rem);

                        }
                        break;
                    }
                case "ASIGNACION_PARA":
                    {
                        if (raiz.ChildNodes.Count() == 4)
                        {
                            string tipo = Ejecutar(hijos[1]).ToString();
                            string nombre = hijos[2].ToString().Replace(" (identificador)", "");
                            Expresion exp = new Expresion(tipo, this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento elem = (Elemento)exp.recorre_expresion(hijos[3]);
                            
                            if (elem != null)
                            {
                                Variable temporal = new Variable(nombre, elem.valor, tipo, ambitosEje.Peek(), false, false);
                                this.LienzoEjecutando.Variables.Add(temporal);//agrego la variable temporal en la lista
                                this.indicePara = this.LienzoEjecutando.Variables.Count() - 1;
                            }
                            else
                            {
                                foreach(ErrorEnAnalisis r in exp.getErroresSemanticos())
                                {
                                    this.errores.Add(r);
                                }
                            }
                        }
                        break;
                    }
                case "ACCIONES":
                    {
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            if (hijos[1].ToString().Contains("+"))
                            {
                                string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                                string tipo = getTipoDeVar(nombre, ambitosEje.Peek());
                                string currentValue = buscaValorDeVar(nombre, ambitosEje.Peek(), tipo);
                                if (currentValue != "")
                                {
                                    if (tipo == "entero")
                                    {
                                        int current = Convert.ToInt32(currentValue);
                                        int nuevoval = current + 1;
                                        if (!buscaVariableYAsignaValor(nombre, nuevoval.ToString(), ambitosEje.Peek(), tipo))
                                        {
                                            ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                            this.errores.Add(error);
                                        }
                                    }
                                    else
                                    {
                                        ErrorEnAnalisis error = new ErrorEnAnalisis("No se puede realizar al aumento a una variable de tipo: " + tipo, "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                        this.errores.Add(error);
                                    }
                                }
                                else
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                    this.errores.Add(error);
                                }
                            }
                            else
                            {
                                string nombre = hijos[0].ToString().Replace(" (identificador)", "");
                                string tipo = getTipoDeVar(nombre, ambitosEje.Peek());
                                string currentValue = buscaValorDeVar(nombre, ambitosEje.Peek(), tipo);
                                if (currentValue != "")
                                {
                                    if (tipo == "entero")
                                    {
                                        int current = Convert.ToInt32(currentValue);
                                        int nuevoval = current - 1;
                                        if (!buscaVariableYAsignaValor(nombre, nuevoval.ToString(), ambitosEje.Peek(), tipo))
                                        {
                                            ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este Ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                            this.errores.Add(error);
                                        }
                                    }
                                    else
                                    {
                                        ErrorEnAnalisis error = new ErrorEnAnalisis("No se puede realizar al aumento a una variable de tipo: " + tipo, "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                        this.errores.Add(error);
                                    }
                                }
                                else
                                {
                                    ErrorEnAnalisis error = new ErrorEnAnalisis("No Existe Variable: ' " + nombre + " ' en este ambito: " + ambitosEje.Peek(), "Error Semantico", hijos[0].Token.Location.Line, hijos[0].Token.Location.Column);
                                    this.errores.Add(error);
                                }
                            }
                        }
                        break;
                    }
                case "PINTAR_PUNTO":
                    {
                        if(raiz.ChildNodes.Count()== 8)//:=pintarp EXPR , EXPR, EXPR, EXPR
                        {
                            //OBTENIENDO PRIMER PARAMETRO
                            Expresion expr1 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento el1 = (Elemento)expr1.recorre_expresion(hijos[1]);
                            //OBTENNIENDO EL SEGUNDO PARAMETRO
                            Expresion expr2 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento el2 = (Elemento)expr2.recorre_expresion(hijos[3]);
                            //OBTENIENDO EL TERCER PARAMETRO
                            Expresion expr3 = new Expresion("cadena", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento el3 = (Elemento)expr3.recorre_expresion(hijos[5]);
                            //OBTENIENDO EL CUARTO PARAMETRO
                            Expresion expr4 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele4 = (Elemento)expr4.recorre_expresion(hijos[7]);
                            if(el1 != null && el2 != null && el3!=null && ele4 != null)
                            {
                                AreaImagen.g = Graphics.FromImage(AreaImagen.map);
                                //AQUI MANDO LOS PARAMETROS QUE NECESITO A LA FUNCION DE PINTAR
                                int posx = Convert.ToInt32(el1.valor);
                                int posy = Convert.ToInt32(el2.valor);
                                string color = el3.valor;
                                int diametro = Convert.ToInt32(ele4.valor);
                                Dibujo.Pintar_Punt(AreaImagen.g, posx, posy, color, diametro);
                                //AreaImagen.p.Refresh();
                            }
                        }
                        break;
                    }
                case "PINTAR_OR":
                    {
                        if(raiz.ChildNodes.Count() == 12)//::= pintarp EXPR, EXPR, EXPR, EXPR, EXPR, EXPR
                        {
                            //OBTENIENDO EL PRIMER PARAMETRO
                            Expresion expr1 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele1 = (Elemento)expr1.recorre_expresion(hijos[1]);
                            //OBTENIENDO EL SEGUNDO PARAMETRO
                            Expresion expr2 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele2 = (Elemento)expr2.recorre_expresion(hijos[3]);
                            //OBTENIENDO EL TERCER PARAMETRO
                            Expresion expr3 = new Expresion("cadena", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele3 = (Elemento)expr3.recorre_expresion(hijos[5]);
                            //OBTENIENDO EL CUARTO PARAMETRO
                            Expresion expr4 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele4 = (Elemento)expr4.recorre_expresion(hijos[7]);
                            //OBTENIENDO EL QUINTO PARAMETRO
                            Expresion expr5 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele5 = (Elemento)expr5.recorre_expresion(hijos[9]);
                            //OBTENIENDO EL SEXTO PARAMETRO
                            Expresion expr6 = new Expresion("caracter", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele6 = (Elemento)expr6.recorre_expresion(hijos[11]);
                            if (ele1 != null && ele2!= null && ele3!=null && ele4!=null && ele5!=null && ele6!=null)
                            {
                                AreaImagen.g = Graphics.FromImage(AreaImagen.map);
                                //AQUI MANDO LOS PARAMETROS PARA QUE DIBUJE
                                int posx = Convert.ToInt32(ele1.valor);
                                int posy = Convert.ToInt32(ele2.valor);
                                string cadena = ele3.valor;
                                int ancho = Convert.ToInt32(ele4.valor);
                                int alto = Convert.ToInt32(ele5.valor);
                                string c = ele6.valor;
                                Dibujo.Pintar_Ovalo_Rectangulo(AreaImagen.g, posx, posy, cadena, ancho, alto, c);
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
                            EjecucionFuncion(hijos[0]);
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
                            EjecucionFuncion(hijos[1]);
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
                            this.varauxiliar = r;
                        }
                        break;
                    }
                case "PINTAR_PUNTO":
                    {
                        if (raiz.ChildNodes.Count() == 8)//:=pintarp EXPR , EXPR, EXPR, EXPR
                        {
                            String ambito = ambitosEje.Peek();
                            //OBTENIENDO PRIMER PARAMETRO
                            Expresion expr1 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento el1 = (Elemento)expr1.recorre_expresion(hijos[1]);
                            //OBTENNIENDO EL SEGUNDO PARAMETRO
                            Expresion expr2 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento el2 = (Elemento)expr2.recorre_expresion(hijos[3]);
                            //OBTENIENDO EL TERCER PARAMETRO
                            Expresion expr3 = new Expresion("cadena", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento el3 = (Elemento)expr3.recorre_expresion(hijos[5]);
                            //OBTENIENDO EL CUARTO PARAMETRO
                            Expresion expr4 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele4 = (Elemento)expr4.recorre_expresion(hijos[7]);
                            if (el1 != null && el2 != null && el3 != null && ele4 != null)
                            {
                                AreaImagen.g = Graphics.FromImage(AreaImagen.map);
                                //AQUI MANDO LOS PARAMETROS QUE NECESITO A LA FUNCION DE PINTAR
                                int posx = Convert.ToInt32(el1.valor);
                                int posy = Convert.ToInt32(el2.valor);
                                string color = el3.valor;
                                int diametro = Convert.ToInt32(ele4.valor);
                                Dibujo.Pintar_Punt(AreaImagen.g, posx, posy, color, diametro);
                                //AreaImagen.p.Refresh();
                            }
                            else
                            {
                                foreach(ErrorEnAnalisis f in expr1.getErroresSemanticos())
                                {
                                    this.errores.Add(f);
                                }
                                foreach (ErrorEnAnalisis f in expr2.getErroresSemanticos())
                                {
                                    this.errores.Add(f);
                                }
                                foreach (ErrorEnAnalisis f in expr3.getErroresSemanticos())
                                {
                                    this.errores.Add(f);
                                }
                                foreach (ErrorEnAnalisis f in expr4.getErroresSemanticos())
                                {
                                    this.errores.Add(f);
                                }
                            }
                        }
                        break;
                    }
                case "PINTAR_OR":
                    {
                        if (raiz.ChildNodes.Count() == 12)//::= pintarp EXPR, EXPR, EXPR, EXPR, EXPR, EXPR
                        {
                            //OBTENIENDO EL PRIMER PARAMETRO
                            Expresion expr1 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele1 = (Elemento)expr1.recorre_expresion(hijos[1]);
                            //OBTENIENDO EL SEGUNDO PARAMETRO
                            Expresion expr2 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele2 = (Elemento)expr2.recorre_expresion(hijos[3]);
                            //OBTENIENDO EL TERCER PARAMETRO
                            Expresion expr3 = new Expresion("cadena", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele3 = (Elemento)expr3.recorre_expresion(hijos[5]);
                            //OBTENIENDO EL CUARTO PARAMETRO
                            Expresion expr4 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele4 = (Elemento)expr4.recorre_expresion(hijos[7]);
                            //OBTENIENDO EL QUINTO PARAMETRO
                            Expresion expr5 = new Expresion("entero", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele5 = (Elemento)expr5.recorre_expresion(hijos[9]);
                            //OBTENIENDO EL SEXTO PARAMETRO
                            Expresion expr6 = new Expresion("caracter", this.LienzoEjecutando.Variables, ambitosEje.Peek());
                            Elemento ele6 = (Elemento)expr6.recorre_expresion(hijos[11]);
                            if (ele1 != null && ele2 != null && ele3 != null && ele4 != null && ele5 != null && ele6 != null)
                            {
                                AreaImagen.g = Graphics.FromImage(AreaImagen.map);
                                //AQUI MANDO LOS PARAMETROS PARA QUE DIBUJE
                                int posx = Convert.ToInt32(ele1.valor);
                                int posy = Convert.ToInt32(ele2.valor);
                                string cadena = ele3.valor;
                                int ancho = Convert.ToInt32(ele4.valor);
                                int alto = Convert.ToInt32(ele5.valor);
                                String c = ele6.valor;
                                Dibujo.Pintar_Ovalo_Rectangulo(AreaImagen.g, posx, posy, cadena, ancho, alto, c);
                            }
                        }
                        break;
                    }
            }
            return retorno;
        }
        #endregion

        #region Loops De Ejecucion
        private void EjecutaCicloMientras(ParseTreeNode Sentencias, ParseTreeNode Condiciones)
        {
            Logica lo = new Logica(this.LienzoEjecutando.Variables, ambitosEje.Peek());
            bool con = (Boolean)lo.EvaluaLogica(Condiciones);
            if (con != null)
            {
                while (con)
                {
                    Ejecutar(Sentencias);
                    con = (Boolean)lo.EvaluaLogica(Condiciones);
                }
            }
        }

        private void EjecutaCicloHacerMientras(ParseTreeNode Sentencias, ParseTreeNode Condiciones)
        {
            Logica lo = new Logica(this.LienzoEjecutando.Variables, ambitosEje.Peek());
            bool con = (Boolean)lo.EvaluaLogica(Condiciones);
            if (con != null)
            {
                do
                {
                    Ejecutar(Sentencias);
                    con = (Boolean)lo.EvaluaLogica(Condiciones);
                } while (con);
            }
        }

        int indicePara;
        private void EjecutaCicloPara(ParseTreeNode Sentencias, ParseTreeNode Condiciones, ParseTreeNode Aumento)
        {
            Logica pr = new Logica(this.LienzoEjecutando.Variables, ambitosEje.Peek());
            bool b = (Boolean)pr.EvaluaLogica(Condiciones);
            while (b)
            {
                Ejecutar(Sentencias);
                Ejecutar(Aumento);
                b = (Boolean)pr.EvaluaLogica(Condiciones);
            }
        }
        #endregion
    }
}
