using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Media;

public class MainMenuScript : Node
{
    public File file = new File();

    //UI Textos
    public RichTextLabel cuadroTexto;
    public RichTextLabel botonOpcionLabel;
    public Label tituloLabel;
    public Sprite fondoSprite;

    Dictionary ParsedData;

    //Animador
    public AnimationPlayer animador;
    public AnimationPlayer animadorBotones;
    

    //Informacion del capitulo 
    public string titulo;
    public int vida = 3;
    public string objeto;
    public string fondo;
    public string musica;


    //Contenidas en array
    public string opcion;
    public string direccion = "capitulo01";
    public string condicion;
    public int indexOpciones = 0;

    //Texto principal
    public string texto;

    //Texto particionado
    public bool existeParticion = false;
    public int indiceParticion = 0;
    public List<string> segmentos;
    public int limiteDeCaracteres = 100;

    public override void _Ready()
    {
        cuadroTexto = GetNode<RichTextLabel>("CuadroTexto");
        botonOpcionLabel = GetNode<RichTextLabel>("RichTextLabel");
        tituloLabel = GetNode<Label>("Titulo");
        fondoSprite = GetNode<Sprite>("Fondo");
        animador = GetNode<AnimationPlayer>("AnimadorTexto");
        animadorBotones = GetNode<AnimationPlayer>("AnimadorBotones");
        animadorBotones = GetNode<AnimationPlayer>("AnimadorBotones");


    }
    public void _on_BotonDerecha_button_down()
    {
        animadorBotones.Stop();
        animadorBotones.Play("boton_derecho");
        CambiarOpciones(true);
    }

    public void _on_BotonIzquierda_button_down()
    {
        animadorBotones.Stop();
        animadorBotones.Play("boton_izquierdo");
        CambiarOpciones(false);
    }
    public void _on_Button_button_down()
    {
        CargarNuevoCapitulo();
    }

    public void _on_BotonOpcion_pressed()
    {
        CargarNuevoCapitulo();
    }

    public void _on_RichTextLabel_meta_clicked()
    {
        CargarNuevoCapitulo();
    }

    private void CargarNuevoCapitulo()
    {
        indexOpciones = 0;
        //Abrimos el archivo y generamos nuestro diccionario godot 
        string nuevoCapitulo;
        nuevoCapitulo = (direccion == "") ? "capitulo00" : direccion;
        file.Open("res://Capitulos/" + nuevoCapitulo + ".txt", File.ModeFlags.Read);
        JSONParseResult test = JSON.Parse(file.GetAsText());
        ParsedData = test.Result as Dictionary;

        //Ordenamos la informacion del texto
        titulo = (string)ParsedData["titulo"];
        vida = int.Parse((ParsedData["vida"].ToString()));
        objeto = (string)ParsedData["objeto"];
        fondo = (string)ParsedData["fondo"];
        musica = (string)ParsedData["musica"];

        //Ordenamos la informacion de los array
        var array = new Godot.Collections.Array { };
        array = (Godot.Collections.Array)ParsedData["opciones"];
        opcion = "[center]" + (string)array[indexOpciones] + "[/center]";
        array = (Godot.Collections.Array)ParsedData["direcciones"];
        direccion = (string)array[indexOpciones];
        array = (Godot.Collections.Array)ParsedData["condiciones"];
        condicion = (string)array[indexOpciones];

        //texto principal
        ParticionarTexto((string)ParsedData["texto"]);

        ActualizarTextos();
    }

    private void CambiarOpciones(bool sentido)
    {
        if (sentido)
        {
            indexOpciones++;
        }
        else
        {
            indexOpciones--;
        }

        //Ordenamos la informacion de los array
        var array = new Godot.Collections.Array { };
        array = (Godot.Collections.Array)ParsedData["opciones"];
        //Si el indice alcanza el limite del array, o es menor a 0, se reinicia.
        indexOpciones = (indexOpciones < 0) ? (array.Count - 1) : indexOpciones;
        indexOpciones = (array.Count == indexOpciones) ? 0 : indexOpciones;

        opcion = "[center]" + (string)array[indexOpciones] + "[/center]";
        array = (Godot.Collections.Array)ParsedData["direcciones"];
        direccion = (string)array[indexOpciones];
        array = (Godot.Collections.Array)ParsedData["condiciones"];
        condicion = (string)array[indexOpciones];

        botonOpcionLabel.BbcodeText = opcion;
    }

    private void ActualizarTextos()
    {
        GD.Print("imprimiendo texto");
        GD.Print(texto);
        cuadroTexto.Text = texto;
        botonOpcionLabel.BbcodeText = opcion;
        tituloLabel.Text = titulo;

        animador.Play("CargandoTextoPrincipal");
        if (fondo != "")
        {
            //fondoSprite.Texture = (Texture)ResourceLoader.Load("res://Images/" + fondo);
        }
    }

    private void ParticionarTexto(string rawText)
    {
        if (rawText.Length < limiteDeCaracteres)
        {
            texto = rawText;
        }
        else
        {
            while (texto.Length > 0)
            {
                int longitudSegmento = Math.Min(limiteDeCaracteres, rawText.Length);
                string segmento = rawText.Substring(0, longitudSegmento);

                if (longitudSegmento == limiteDeCaracteres)
                {
                    int ultimoEspacio = segmento.LastIndexOf(" ");

                    if (ultimoEspacio != -1)
                    {
                        segmento = texto.Substring(0, ultimoEspacio);
                        longitudSegmento = ultimoEspacio;
                    }
                }

                segmentos.Add(segmento.Trim());
                texto = texto.Substring(longitudSegmento).Trim();
            }
        }

        segmentos = new List<string>();
    }

}