using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Prueba_Gramatica_IRONY_Proyecto2.Analizador
{
    class Gramatica : Grammar
    {
        public Gramatica() : base(caseSensitive : true)
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
            PROCEDIMIENTOS = new NonTerminal("PROCEDIMIENTOS"),
            PARAMETROS = new NonTerminal("PARAMETROS"),
            FUNCIONES = new NonTerminal("FUNCIONES"),
            RETORNAR = new NonTerminal("RETORNAR");
            #endregion

            #region Gramatica
            //------------------GRAMATICA DEL CUERPO GENERAL DEL LIENZO--------------------------

            INICIO.Rule = VISIBILIDAD + Lienzo + identificador + EXTIENDE + opk + CUERPOLIENZO + clk;
            VISIBILIDAD.Rule = publico
                            | privado
                            | Empty;
            EXTIENDE.Rule = extiende + LISTADOEX
                            | Empty;
            LISTADOEX.Rule = LISTADOEX + com + LISTADOEX
                            | identificador;

            //------------------CUERPO DEL LIENZO ------------------------------------------------

            CUERPOLIENZO.Rule = PRINCIPAL + OTROS;//RECORDAR PONER VARIABLES GLOBALES ANTES DEL METODO PRINCIPAL
            PRINCIPAL.Rule = principal + opp + clp + opk + clk
                            | Empty;
            OTROS.Rule = VARGLOBALES + OTROS
                         | PROCEDIMIENTOS + OTROS
                         | Empty;

            //------------------DECLARACION DE VARIABLES GLOBALES --------------------------------
            VARGLOBALES.Rule = CONSERVAR + vr + TIPO + TIPOASIGNACION + finSent;
            TIPOASIGNACION.Rule = ASIGNACION
                            | arreglo + ASIGNACIONARR + DIMENSIONES + LLENADOARR;
            CONSERVAR.Rule = conservar
                            | Empty;
            TIPO.Rule = Ent
                       | doble
                       | Booleano
                       | caract
                       | cad;
            ASIGNACION.Rule = ASIGNACION + com + ASIGNACION
                       | identificador
                       | identificador + igual + EXPR;

            ASIGNACIONARR.Rule = ASIGNACIONARR + com + ASIGNACIONARR
                       | identificador;

            DIMENSIONES.Rule = opb + EXPR + clb + DIMENSIONES
                       | opb + EXPR + clb;

            LLENADOARR.Rule = opl + LISTADOARR + cll
                       | Empty;

            LISTADOARR.Rule = LISTADOARR + com + LISTADOARR
                       | opl + LISTADOARR + cll
                       | EXPR;

            //EXPRESIONES PARA OPERACIONES
            EXPR.Rule = E;

            E.Rule = E + mas + T
                   | E + menos + T
                   | T;

            T.Rule = T + por + G
                   | T + div + G
                   | G;

            G.Rule = G + pot + F
                   | F;

            F.Rule = opp + E + clp
                   | identificador
                   | entero
                   | identificador + DIMENSIONES
                   | dec
                   | verd
                   | fals
                   | caracter
                   | cadena;

            //GRAMATICA DE PROCEDIMIENTOS

            PROCEDIMIENTOS.Rule = CONSERVAR + identificador + opp + PARAMETROS + clp + opk + clk;

            PARAMETROS.Rule = PARAMETROS + com + PARAMETROS
                            | TIPO + identificador
                            | Empty;

            // GRAMATICA DE FUNCIONES

            FUNCIONES.Rule = CONSERVAR + TIPO + identificador + opp + PARAMETROS + clp + opk + RETORNAR + finSent + clk;

            RETORNAR.Rule = identificador
                            | EXPR;
            #endregion

            #region Preferencias
            this.Root = INICIO;
            #endregion
        }
    }
}
