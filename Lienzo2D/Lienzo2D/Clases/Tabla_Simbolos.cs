﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Lienzo2D.Clases
{
    class Tabla_Simbolos
    {
        List<Simbolo> Tabla = new List<Simbolo>();
        Stack<String> ambitos = new Stack<string>();
        List<Simbolo> auxiliar = new List<Simbolo>();
        ParseTreeNode raizAST;
        public Tabla_Simbolos(ParseTreeNode raiz)
        {
            this.raizAST = raiz;
        }

        public List<Simbolo> getTable()
        {
            return this.Tabla;
        }

        public void generarme_tabla()
        {
            generarTabla(this.raizAST);
        }

        public object generarTabla(ParseTreeNode raiz)
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
                            visi = visi.Replace("(Keyword)", "");
                            string tipo = hijos[1].ToString().Replace("(Keyword)", "");
                            string nombre = hijos[2].ToString().Replace("(identificador)", "");
                            ambitos.Push(nombre);
                            Simbolo nuevo = new Simbolo(nombre, tipo, visi, "No Aplica", ambitos.Peek() , false);
                            Tabla.Add(nuevo);
                            //ME MUEVO AL CUERPO DEL LIENZO
                            generarTabla(hijos[4]);
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
                            string nombre = hijos[0].ToString().Replace("(Keyword)", "");
                            ambitos.Push(nombre);
                            Simbolo nuevo = new Simbolo(nombre, "Main", "No Aplica", "No Aplica", ambitos.Peek(),false);
                            Tabla.Add(nuevo);
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
                            //me va a retornar un simbolo en la prducción tipo asignación
                            generarTabla(hijos[3]);//mete los valores a la lista auxiliar
                            string ambito = ambitos.Peek();
                            foreach(Simbolo c in auxiliar)
                            {//aqui podría hacer las comprobaciones semánticas
                                c.visibilidad = visibi;
                                c.conservar = conservar;
                                c.tipo = tipo;
                                c.ambito = ambito;
                                Tabla.Add(c);
                            }
                            //LIBERO LA LISTA
                            auxiliar.Clear();

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
                case "TIPO":
                    {
                        string tipo = hijos[0].ToString().Replace("(Keyword)","");
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
                        break;
                    }
                case "ASIGNACION":
                    {
                        if (raiz.ChildNodes.Count() == 1)
                        {
                            Simbolo aux = new Simbolo();
                            aux.nombre = hijos[0].ToString().Replace("(identificador)","");
                            aux.valor = null;
                            auxiliar.Add(aux);
                        }
                        if(raiz.ChildNodes.Count() == 2)
                        {
                            Simbolo aux = new Simbolo();
                            aux.nombre = hijos[0].ToString().Replace("(identificador)", "");
                            //aux.valor = a lo que retorne expresiones
                            //añadir a lista auxiliar
                        }
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            generarTabla(hijos[0]);
                            generarTabla(hijos[2]);
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
        

    }
}
