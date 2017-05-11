using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;

namespace Lienzo2D.Analizador
{
    class Gramatica : Grammar
    {
        List<ErrorEnAnalisis> errores = new List<ErrorEnAnalisis>();

        public List<ErrorEnAnalisis> getErrores()
        {
            return this.errores;
        }

        public Gramatica() : base(caseSensitive: true)
        {
            #region ER
            RegexBasedTerminal entero = new RegexBasedTerminal("ent", "[0-9]+");
            RegexBasedTerminal dec = new RegexBasedTerminal("deci", "[0-9]+\\.+[0-9]+");
            RegexBasedTerminal verd = new RegexBasedTerminal("true", "true");
            RegexBasedTerminal fals = new RegexBasedTerminal("false", "false");
            RegexBasedTerminal caracter = new RegexBasedTerminal("carac", "\'.?\'");
            IdentifierTerminal identificador = new IdentifierTerminal("identificador");
            StringLiteral cadena = new StringLiteral("cade", "\"");
            CommentTerminal linea = new CommentTerminal("cometario_linea", ">>", "\n");
            CommentTerminal multlinea = new CommentTerminal("comentario_multi", "<-", "->");
            //ME HACEN FALTA LOS CARACTERES ESPECIALES
            RegexBasedTerminal comillaSimp = new RegexBasedTerminal("comillaSim", "^'");
            RegexBasedTerminal acentoCircun = new RegexBasedTerminal("acentoCirc", "^^");
            RegexBasedTerminal tab = new RegexBasedTerminal("tab", "#t");
            RegexBasedTerminal salto = new RegexBasedTerminal("salto", "#n");
            RegexBasedTerminal comillasDoble = new RegexBasedTerminal("comillasDob", "#\"");
            RegexBasedTerminal numeral = new RegexBasedTerminal("numeral", "##");
            #endregion

            #region Terminales
            //SIGNOS DE AGRUPACIÓN
            var opk = ToTerm("¿");
            var clk = ToTerm("?");
            var opp = ToTerm("(");
            var clp = ToTerm(")");
            var com = ToTerm(",");
            var pc = ToTerm(";");
            var opl = ToTerm("{");
            var cll = ToTerm("}");
            var opb = ToTerm("[");
            var clb = ToTerm("]");
            //SIGNOS RELACIONALES
            var igualacion = ToTerm("==");
            var diferenciacion = ToTerm("!=");
            var menorque = ToTerm("<");
            var menorIgual = ToTerm("<=");
            var mayorque = ToTerm(">");
            var mayorIgual = ToTerm(">=");
            //SIGNOS DE OPERADORES LÓGICO
            var or = ToTerm("||");
            var and = ToTerm("&&");
            var nand = ToTerm("!&&");
            var nor = ToTerm("!||");
            var xor = ToTerm("&|");
            var not = ToTerm("!");
            //OPERADORES
            var mas = ToTerm("+");
            var menos = ToTerm("-");
            var por = ToTerm("*");
            var div = ToTerm("/");
            var pot = ToTerm("^");
            var finSent = ToTerm("$");
            var igual = ToTerm("=");
            //PALABRAS RESERVADAS
            var publico = ToTerm("publico");
            var privado = ToTerm("privado");
            var conservar = ToTerm("Conservar");
            var Lienzo = ToTerm("Lienzo");
            var extiende = ToTerm("extiende");
            var arreglo = ToTerm("arreglo");
            var doble = ToTerm("doble");
            var Ent = ToTerm("entero");
            var Booleano = ToTerm("boolean");
            var caract = ToTerm("caracter");
            var cad = ToTerm("cadena");
            var vr = ToTerm("var");
            var ret = ToTerm("retorna");
            //PALABRAS RESERVADAS SENTENCIAS:
            var si = ToTerm("si");
            var sino = ToTerm("sino");
            var para = ToTerm("para");
            var mientras = ToTerm("mientras");
            var hacer = ToTerm("hacer");
            //FUNCIONES NATIVAS PALABRAS RESERVADAS
            var pintarp = ToTerm("Pintar_P");
            var pintaror = ToTerm("Pintar_OR");
            //PALABRA RESERVADA DEL METODO PRINCIPAL
            var principal = ToTerm("Principal");
            #endregion

            #region No Terminales
            NonTerminal INICIO = new NonTerminal("INICIO"),
            VISIBILIDAD = new NonTerminal("VISIBILIDAD"),
            EXTIENDE = new NonTerminal("EXTIENDE"),
            LISTADOEX = new NonTerminal("LISTADOEX"),
            CUERPOLIENZO = new NonTerminal("CUERPOLIENZO"),
            PRINCIPAL = new NonTerminal("PRINCIPAL"),
            OTROS = new NonTerminal("OTROS"),
            VARGLOBALES = new NonTerminal("VARGLOBALES"),
            CONSERVAR = new NonTerminal("CONSERVAR"),
            TIPO = new NonTerminal("TIPO"),
            TIPOASIGNACION = new NonTerminal("TIPOASIGNACION"),
            ASIGNACION = new NonTerminal("ASIGNACION"),
            ASIGNACIONARR = new NonTerminal("ASIGNACIONARR"),
            DIMENSIONES = new NonTerminal("DIMENSIONES"),
            LLENADOARR = new NonTerminal("LLENADOARR"),
            LISTADOARR = new NonTerminal("LISTADOARR"),
            EXPR = new NonTerminal("EXPR"),
            E = new NonTerminal("E"),
            T = new NonTerminal("T"),
            G = new NonTerminal("G"),
            F = new NonTerminal("F"),
            PRO_FUNC = new NonTerminal("PRO_FUNC"),
            PRO_FUNCP = new NonTerminal("PRO_FUNCP"),
            PROCEDIMIENTOS = new NonTerminal("PROCEDIMIENTOS"),
            PARAMETROS = new NonTerminal("PARAMETROS"),
            FUNCIONES = new NonTerminal("FUNCIONES"),
            RETORNAR = new NonTerminal("RETORNAR"),
            VARLOCALES = new NonTerminal("VARLOCARES"),
            ASIGNAVAR = new NonTerminal("ASIGNAVAR"),
            DIMOPCIONAL = new NonTerminal("DIMOPCIONAL"),
            SENTENCIAS = new NonTerminal("SENTENCIAS"),
            SENTENCIASP = new NonTerminal("SENTENCIASP"),
            SENTENCIA_SI = new NonTerminal("SENTENCIA_SI"),
            CONDICIONES = new NonTerminal("CONDICIONES"),
            SINO = new NonTerminal("SINO"),
            EXPLOGICA = new NonTerminal("EXPLOGICA"),
            B = new NonTerminal("B"),
            C = new NonTerminal("C"),
            D = new NonTerminal("D"),
            RELACIONALES = new NonTerminal("RELACIONALES"),
            A = new NonTerminal("A"),
            SENTENCIA_PARA = new NonTerminal("SENTENCIA_PARA"),
            ASIGNACIONPARA = new NonTerminal("ASIGNACION_PARA"),
            ACCIONES = new NonTerminal("ACCIONES"),
            SENTENCIA_MIENTRAS = new NonTerminal("SENTENCIA_MIENTRAS"),
            SENTENCIA_HACER = new NonTerminal("SENTENCIA_HACER"),
            PINTAR_PUNTO = new NonTerminal("PINTAR_PUNTO"),
            PINTAR_OR = new NonTerminal("PINTAR_OR"),
            FUN_PRO = new NonTerminal("FUN_PRO"),
            SENTE_AU = new NonTerminal("SENTE_AU_DEC"),
            SENTE_DEC = new NonTerminal("SENTE_DEC"),
            DIM = new NonTerminal("DIM"),
            RESULTADOFUN = new NonTerminal("RESULTADOFUN"),
            LISTADOEXPRE = new NonTerminal("LISTADOEXPRE"),
            EXPRPRIMA = new NonTerminal("EXPRPRIMA"),
            EP = new NonTerminal("EP"),
            TP = new NonTerminal("TP"),
            GP = new NonTerminal("GP"),
            FP = new NonTerminal("FP"),
            CONDICIONESPRIMA = new NonTerminal("CONDICIONESPRIMA");
            #endregion

            #region Gramatica
            //------------------GRAMATICA DEL CUERPO GENERAL DEL LIENZO--------------------------

            INICIO.Rule = VISIBILIDAD + Lienzo + identificador + EXTIENDE + opk + CUERPOLIENZO + clk
                         | SyntaxError + clk;
            VISIBILIDAD.Rule = publico
                            | privado
                            | SyntaxError
                            | Empty;
            EXTIENDE.Rule = extiende + LISTADOEX
                            |SyntaxError
                            | Empty;
            LISTADOEX.Rule = LISTADOEX + com + LISTADOEX
                            | SyntaxError
                            | identificador;

            //------------------CUERPO DEL LIENZO ------------------------------------------------
            //RECORDAR PONER VARIABLES GLOBALES ANTES DEL METODO PRINCIPAL
            CUERPOLIENZO.Rule = PRINCIPAL + OTROS
                             | VARGLOBALES + CUERPOLIENZO
                             | PRO_FUNC + CUERPOLIENZO
                             //| SyntaxError + finSent
                             | Empty;

            PRINCIPAL.Rule = principal + opp + clp + opk + SENTENCIAS + clk
                            | SyntaxError + clk
                            | Empty;

            OTROS.Rule = VARGLOBALES + OTROS
                         | PRO_FUNC + OTROS
                         | SyntaxError
                         | Empty;

            //------------------DECLARACION DE VARIABLES GLOBALES --------------------------------
            VARGLOBALES.Rule = CONSERVAR + vr + TIPO + TIPOASIGNACION + finSent
                            | SyntaxError + finSent;
            TIPOASIGNACION.Rule = ASIGNACION
                            |arreglo + ASIGNACIONARR + DIMENSIONES + LLENADOARR
                            |SyntaxError;
            CONSERVAR.Rule = conservar
                            | SyntaxError
                            | Empty;
            TIPO.Rule = Ent
                       |doble
                       |Booleano
                       |caract
                       |cad
                       |SyntaxError;
            ASIGNACION.Rule = ASIGNACION + com + ASIGNACION
                       | identificador
                       | identificador + igual + EXPR
                       |SyntaxError;

            ASIGNACIONARR.Rule = ASIGNACIONARR + com + ASIGNACIONARR
                       | identificador
                       |SyntaxError;

            DIMENSIONES.Rule = opb + EXPR + clb + DIMENSIONES
                       | opb + EXPR + clb
                       | SyntaxError;

            LLENADOARR.Rule = igual + opl + LISTADOARR + cll
                       | igual + RESULTADOFUN
                       | SyntaxError
                       | Empty;

            LISTADOARR.Rule = LISTADOARR + com + LISTADOARR
                       | opl + LISTADOARR + cll
                       | SyntaxError
                       | EXPR;

            //EXPRESIONES PARA OPERACIONES
            EXPR.Rule = E
                |SyntaxError;

            E.Rule = E + mas + T
                   | E + menos + T
                   | T
                   | SyntaxError;//agregado

            T.Rule = T + por + G
                   | T + div + G
                   | G
                   | SyntaxError;

            G.Rule = G + pot + F
                   | F
                   | SyntaxError;

            F.Rule = opp + E + clp
                   | identificador
                   | entero
                   | identificador + DIMENSIONES
                   | dec
                   | verd
                   | fals
                   | caracter
                   | RESULTADOFUN
                   | cadena
                   | SyntaxError;

            EXPRPRIMA.Rule = EP
                    |SyntaxError;

            EP.Rule = EP + mas + TP
                    | EP + menos + TP
                    | TP
                    |SyntaxError;

            TP.Rule = TP + por + GP
                    | TP + div + GP
                    | GP
                    |SyntaxError;

            GP.Rule = GP + pot + FP
                    | FP
                    |SyntaxError;

            FP.Rule = opp + EP + clp
                   | opp + RELACIONALES + clp
                   | opp + EXPLOGICA + clp
                   | identificador
                   | entero
                   | identificador + DIMENSIONES
                   | dec
                   | verd
                   | fals
                   | caracter
                   | RESULTADOFUN
                   | cadena
                   | SyntaxError;


            //ELIGE ENTRE PROCEDIMIENTOS Y FUNCIONES

            PRO_FUNC.Rule = CONSERVAR + VISIBILIDAD + PRO_FUNCP
                            |SyntaxError;

            PRO_FUNCP.Rule = PROCEDIMIENTOS
                            | TIPO + FUNCIONES
                            |SyntaxError;

            //GRAMATICA DE PROCEDIMIENTOS

            PROCEDIMIENTOS.Rule = identificador + opp + PARAMETROS + clp + opk + SENTENCIAS + clk
                                | SyntaxError+clk;

            PARAMETROS.Rule = PARAMETROS + com + PARAMETROS
                            | TIPO + identificador
                            | SyntaxError
                            | Empty;

            // GRAMATICA DE FUNCIONES

            FUNCIONES.Rule = DIM + identificador + opp + PARAMETROS + clp + opk + SENTENCIASP + clk
                            | SyntaxError + clk;

            DIM.Rule = opb + clb + DIM
                      |SyntaxError
                      | Empty;

            RETORNAR.Rule = ret + identificador
                            | ret + EXPR
                            |SyntaxError;

            // GRAMATICA DE ASINACION DE VALOR DE VARIABLES
            ASIGNAVAR.Rule = identificador + DIMOPCIONAL + igual + EXPR + finSent
                            | SyntaxError + finSent;

            DIMOPCIONAL.Rule = DIMENSIONES
                            | SyntaxError
                            | Empty;

            //VARIABLES LOCALES 

            VARLOCALES.Rule = CONSERVAR + vr + TIPO + TIPOASIGNACION + finSent
                            | SyntaxError + finSent;

            //SENTENCIAS DENTRO DE LOS PROCEDIMIENTOS
            SENTENCIAS.Rule = VARLOCALES + SENTENCIAS
                            | ASIGNAVAR + SENTENCIAS
                            | SENTENCIA_SI + SENTENCIAS
                            | SENTENCIA_MIENTRAS + SENTENCIAS
                            | SENTENCIA_PARA + SENTENCIAS
                            | SENTENCIA_HACER + SENTENCIAS
                            | PINTAR_PUNTO + SENTENCIAS
                            | PINTAR_OR + SENTENCIAS
                            | FUN_PRO + SENTENCIAS
                            | SENTE_AU + SENTENCIAS
                            | SENTE_DEC + SENTENCIAS
                            | SyntaxError + finSent
                            | Empty;

            //SENTENCIAS DENTRO DE FUNCIONES
            SENTENCIASP.Rule = VARLOCALES + SENTENCIASP
                            | ASIGNAVAR + SENTENCIASP
                            | SENTENCIA_SI + SENTENCIASP
                            | SENTENCIA_MIENTRAS + SENTENCIASP
                            | SENTENCIA_PARA + SENTENCIASP
                            | SENTENCIA_HACER + SENTENCIASP
                            | PINTAR_PUNTO + SENTENCIASP
                            | PINTAR_OR + SENTENCIASP
                            | FUN_PRO + SENTENCIASP
                            | SENTE_AU + SENTENCIASP
                            | SENTE_DEC + SENTENCIASP
                            | RETORNAR + finSent + SENTENCIASP
                            | SyntaxError + finSent
                            | Empty;

            //SENTENCIA SI
            SENTENCIA_SI.Rule = si + opp + CONDICIONES + clp + opk + SENTENCIAS + clk + SINO
                                | SyntaxError;


            SINO.Rule = sino + opk + SENTENCIAS + clk
                      | SyntaxError + clk
                      | Empty;

            CONDICIONES.Rule = EXPLOGICA
                            |SyntaxError;

            //EXPRESIONES LOGICAS
            EXPLOGICA.Rule = B
                            | SyntaxError;

            B.Rule = B + or + C
                   | B + nor + C
                   | B + xor + C
                   | C
                   | SyntaxError;

            C.Rule = C + and + D
                   | C + nand + D
                   | D
                   |SyntaxError;

            D.Rule = not + RELACIONALES
                   | RELACIONALES
                   | SyntaxError;

            //EXPRESIONES RELACIONALES
            RELACIONALES.Rule = A
                               |SyntaxError;

            A.Rule = A + igualacion + A
                   | A + diferenciacion + A
                   | A + menorque + A
                   | A + mayorIgual + A
                   | A + menorIgual + A
                   | A + mayorque + A
                   | EXPRPRIMA
                   |SyntaxError;

            //SENTENCIA PARA 
            SENTENCIA_PARA.Rule = para + opp + ASIGNACIONPARA + pc + RELACIONALES + pc + ACCIONES + clp + opk + SENTENCIAS + clk
                                | SyntaxError;
            ASIGNACIONPARA.Rule = vr + TIPO + identificador + igual + EXPR
                                | identificador + igual + EXPR
                                | SyntaxError;
            ACCIONES.Rule = identificador + mas + mas
                            | identificador + menos + menos
                            | SyntaxError;

            //SENTENCIA MIENTRAS
            SENTENCIA_MIENTRAS.Rule = mientras + opp + CONDICIONES + clp + opk + SENTENCIAS + clk
                                    | SyntaxError;

            //SENTENCIAS HACER 
            SENTENCIA_HACER.Rule = hacer + opk + SENTENCIAS + clk + mientras + opp + CONDICIONES + clp
                                | SyntaxError;

            //FUNCIONES NATIVAS DEL LENGUAJE
            PINTAR_PUNTO.Rule = pintarp + opp + EXPR + com + EXPR + com + EXPR + com + EXPR + clp + finSent
                              | SyntaxError + finSent;

            PINTAR_OR.Rule = pintaror + opp + EXPR + com + EXPR + com + EXPR + com + EXPR + com + EXPR + com + EXPR + clp + finSent
                              | SyntaxError + finSent;

            //LLAMADAS A FUNCIONES
            FUN_PRO.Rule = identificador + opp + LISTADOEXPRE + clp + finSent
                            | SyntaxError + finSent;

            //SENTENCIAS DE AUMENTOS Y DECREMENTOS DE VARIABLES
            SENTE_AU.Rule = identificador + mas + mas + finSent
                            | identificador + mas + igual + EXPR + finSent
                            | SyntaxError + finSent;

            SENTE_DEC.Rule = identificador + menos + menos + finSent
                            | identificador + menos + igual + EXPR + finSent
                            | SyntaxError + finSent;

            //RESULTADO FUNCIONES
            RESULTADOFUN.Rule = identificador + opp + LISTADOEXPRE + clp
                             | SyntaxError;

            //LISTADO DE EXPRESIONES QUE SE RECIBEN
            LISTADOEXPRE.Rule = LISTADOEXPRE + com + LISTADOEXPRE
                               | EXPR
                               | Empty
                               | SyntaxError;

            #endregion

            #region Preferencias
            this.MarkPunctuation("¿", "?","(",")","$","=");
            NonGrammarTerminals.Add(linea);
            NonGrammarTerminals.Add(multlinea);
            this.Root = INICIO;
            //LanguageFlags = LanguageFlags.CreateAst;
            #endregion

            
        }

        public override void ReportParseError(ParsingContext context)
        {
            String error = (String)context.CurrentToken.ValueString;
            String tipo;
            int fila;
            int columna;
            if (error.Contains("Invalid character"))
            {
                tipo = "Error Lexico";
                string delimStr = ":";
                char[] delimiter = delimStr.ToCharArray();
                string[] division = error.Split(delimiter, 2);
                division = division[1].Split('.');
                error = "Caracter Invalido" + division[0];
            }
            else
            {
                tipo = "Error Sintactico";
                error = "Token No esperado, se encontro: " + error;
            }
            fila = context.Source.Location.Line;
            columna = context.Source.Location.Column;
            ErrorEnAnalisis err = new ErrorEnAnalisis(error, tipo, fila, columna);
            errores.Add(err);
            base.ReportParseError(context);
        }
    }
}
