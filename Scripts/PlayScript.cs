using Godot;
using Godot.Collections;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

public class PlayScript : Node
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
    public List<string> sonidosBoton = new List<string> { "progress.mp3", "progress_two.mp3", "progress_end.mp3", "progress_two.mp3" };
    public int indexSonidos = 0;
    public AudioStreamPlayer buttonMusicPLayer;
	public Button buttonLeft;
	public Button buttonRight;
	public bool isHidden = false;
	public bool primeraCarga = true;

    //Informacion del capitulo 
    public string titulo;
	public int vida = 3;
	public string objeto;
	public string fondo;
	public string musica;
	public string personaje;
	public bool opcionesActivas = false;

	//Contenidas en array
	public string opcion;
	public string direccion = "tutorial";
	public string condicion;
	public int indexOpciones = 0;

	//Texto principal
	public string texto;

	//Texto particionado
	public bool existeParticion = false;
	public int indiceParticion = 0;
	public int indiceParticionMax = 0;
	public List<string> segmentosCargados;
	public int limiteDeCaracteres = 141;
	public string opcionFija = "[center]Continuar[/center]";

    //Botones
    public Button botonVolver;

    //Musica
    public AudioStreamPlayer BaseAudio;
    public bool musicOn = true;
    public string musicStock;
    public Sprite audioImg;
    public Button botonMusica;

    public override void _Ready()
	{
		cuadroTexto = GetNode<RichTextLabel>("CuadroTexto");
		botonOpcionLabel = GetNode<RichTextLabel>("RichTextLabel");
		tituloLabel = GetNode<Label>("Titulo");
		fondoSprite = GetNode<Sprite>("Fondo");
		animador = GetNode<AnimationPlayer>("AnimadorTexto");
		animadorBotones = GetNode<AnimationPlayer>("AnimadorBotones");
		buttonMusicPLayer = GetNode<AudioStreamPlayer>("ButtonMusicPLayer");
        BaseAudio = GetNode<AudioStreamPlayer>("BaseAudio");

        animador.Play("CargandoTextoPrincipal");
		botonVolver = GetNode<Button>("BotonVolver");
        botonMusica = GetNode<Button>("BotonMusica");
        audioImg = botonMusica.GetNode<Sprite>("MusicPng"); 

        String capituloGuardado = CargarCapitulo();
        if (capituloGuardado != "")
        {
            direccion = capituloGuardado;

		}
		else
		{
			capituloGuardado = "tutorial";

        }
        CargarNuevoCapitulo();

		buttonLeft = GetNode<Button>("BotonIzquierda");
        buttonRight = GetNode<Button>("BotonDerecha");
		

    }

    public void ReproducirSonidoBoton()
    {
        string audioFile = sonidosBoton[indexSonidos];
        AudioStream audio = (AudioStream)GD.Load("res://Sonido/" + audioFile);
        buttonMusicPLayer.Stream = audio;
        buttonMusicPLayer.Play();
        indexSonidos++;
        if (indexSonidos == 4) { indexSonidos = 0; };
    }

    public async Task FadeOutIn(float fadeTime = 0.5f, float silenceTime = 0.2f)
    {
        if (BaseAudio == null)
            return;

		if (!musicOn)
			return;

        float originalVolume = BaseAudio.VolumeDb;
        var tweenOut = GetTree().CreateTween();
        tweenOut.TweenProperty(BaseAudio, "volume_db", -80f, fadeTime);
        await ToSignal(tweenOut, "finished");
        await ToSignal(GetTree().CreateTimer(silenceTime), "timeout");
        var tweenIn = GetTree().CreateTween();
        tweenIn.TweenProperty(BaseAudio, "volume_db", originalVolume, fadeTime);
    }
    public async void ReproducirMusica(string sound)
	{
        if (musicStock != sound)
        {
            AudioStream musica;
			musicStock = sound;
            await FadeOutIn();
            switch (sound)
			{
				case "stop":
					BaseAudio.Stop();
					break;
				case "planicies_enredadera.mp3":
                    musicStock = "incendio.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "incendio.mp3");
					BaseAudio.Stream = musica;
					BaseAudio.Play();
					break;
				case "rio_final.mp3":
                    musicStock = "incendio.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "incendio.mp3");
					BaseAudio.Stream = musica;
					BaseAudio.Play();
					break;
				case "rio_final_ataque.mp3":
                    musicStock = "incendio.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "incendio.mp3");
					BaseAudio.Stream = musica;
					BaseAudio.Play();
					break;
				case "rio_final_espera.mp3":
                    musicStock = "la_planta.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "la_planta.mp3");
					BaseAudio.Stream = musica;
					BaseAudio.Play();
					break;
				case "final.mp3":
                    musicStock = "la_planta.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "la_planta.mp3");
					BaseAudio.Stream = musica;
					BaseAudio.Play();
					break;
                case "corrientes.mp3":
                    musicStock = "incendio.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "incendio.mp3");
                    BaseAudio.Stream = musica;
                    BaseAudio.Play();
                    break;
                case "a_salvo.mp3":
                    musicStock = "rio.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "rio.mp3");
                    BaseAudio.Stream = musica;
                    BaseAudio.Play();
                    break;
                case "incendio_01.mp3":
                    musicStock = "incendio.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "incendio.mp3");
                    BaseAudio.Stream = musica;
                    BaseAudio.Play();
                    break;
                case "mapache.mp3":
                    musicStock = "bosque.mp3";
                    musica = (AudioStream)GD.Load("res://Sonido/" + "bosque.mp3");
                    BaseAudio.Stream = musica;
                    BaseAudio.Play();
                    break;
                default:
					musica = (AudioStream)GD.Load("res://Sonido/" + sound );
					BaseAudio.Stream = musica;
					BaseAudio.Play();
					break;
			}
        }
    }
    public void _on_BotonMusica_pressed()
	{
		if (musicOn)
		{
            audioImg.Texture = (Texture)ResourceLoader.Load("res://Images/music-off.png");
            musicOn = false;
			BaseAudio.SetVolumeDb(-100f);
        }
		else
		{
            audioImg.Texture = (Texture)ResourceLoader.Load("res://Images/music-on.png");
            musicOn = true;
            BaseAudio.SetVolumeDb(-7.8f);
        }

		


    }
    public void _on_BotonDerecha_button_down()
	{
		if (opcionesActivas)
		{
			animadorBotones.Stop();
			animadorBotones.Play("boton_derecho");
			CambiarOpciones(true);
		}
	}

	public void _on_BotonIzquierda_button_down()
	{
		if (opcionesActivas) 
		{
			animadorBotones.Stop();
			animadorBotones.Play("boton_izquierdo");
			CambiarOpciones(false);
		}
	}

	public void _on_BotonOpcion_pressed()
	{
		GD.Print(existeParticion);
		if (existeParticion)
		{
			ValidarParticion();
			ActualizarTextos();
		}
		else
		{
			indiceParticion = 0;
			CargarNuevoCapitulo();
		}
		
	}

	public void _on_BotonVolver_pressed()
	{
        ActivarOpciones(0);
		existeParticion = true;
        indiceParticion--;
		ActualizarTextos();
		if (indiceParticion == 0)
		{
			botonVolver.Visible = false;
		};
	}

	private void ContinuarCapitulo()
	{
		ActualizarTextos();
	}
	private void CargarNuevoCapitulo()
	{
		indexOpciones = 0;
		//Abrimos el archivo y generamos nuestro diccionario godot 
		string nuevoCapitulo;
		GD.Print(direccion);
		nuevoCapitulo = (direccion == "") ? "tutorial" : direccion;
		GD.Print(nuevoCapitulo);
		file.Open("res://Capitulos/" + nuevoCapitulo + ".txt", File.ModeFlags.Read);
		try
		{
			JSONParseResult test = JSON.Parse(file.GetAsText());
			ParsedData = test.Result as Dictionary;

			//Ordenamos la informacion del texto
			titulo = (string)ParsedData["titulo"];
			vida = Convert.ToInt32(ParsedData["vida"]);
			objeto = (string)ParsedData["objeto"];
			fondo = (string)ParsedData["fondo"];
			musica = (string)ParsedData["musica"];
			personaje = (string)ParsedData["personaje"];

            //Ordenamos la informacion de los array
            var array = new Godot.Collections.Array { };
			array = (Godot.Collections.Array)ParsedData["opciones"];
			opcion = "[center]" + (string)array[indexOpciones] + "[/center]";
			ActivarOpciones(array.Count);
			array = (Godot.Collections.Array)ParsedData["direcciones"];
			direccion = (string)array[indexOpciones];
			array = (Godot.Collections.Array)ParsedData["condiciones"];
			condicion = (string)array[indexOpciones];

			//texto principal
			ParticionarTexto((string)ParsedData["texto"]);
			GuardarCapitulo(nuevoCapitulo);
            ReproducirMusica(musica);

        }
		catch (Exception e)
		{
			GD.Print(e.ToString());
		}


		ActualizarTextos();
	}

	private void ActivarOpciones(int activar = 0)
	{
		if (activar > 1) 
		{ 
			opcionesActivas = true;
		}
		else
		{
			opcionesActivas = false;
		}
	}
	private void CambiarOpciones(bool sentido)
	{
		ReproducirSonidoBoton();

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

	private void EsconderBotones(String texto)
	{
		GD.Print(texto);
		if(primeraCarga && (texto != "[center]Continuar[/center]" || texto != "[center]Volver a intentar[/center] " || texto != "[center]Sigues tu camino[/center]"))
        {
            isHidden = true;
			primeraCarga = false;
        }

		if (!isHidden && (texto == "[center]Continuar[/center]" || texto == "[center]Volver a intentar[/center]" || texto == "[center]Sigues tu camino[/center]"))
		{
            GD.Print("ocultando texto");
            isHidden = true;
			animadorBotones.PlayBackwards("BotonesIn");
		}
		else if(isHidden && texto != "[center]Continuar[/center]")
		{
            GD.Print("mostrando texto");
            animadorBotones.Play("BotonesIn");
            isHidden = false;
        }
    }

    private void ActualizarTextos()
	{
		tituloLabel.Text = titulo;
		if (indiceParticion < indiceParticionMax-1)
		{

            botonOpcionLabel.BbcodeText = opcionFija;
			cuadroTexto.Text = (segmentosCargados[indiceParticion] + "...").Replace("&", "");
            EsconderBotones(opcionFija);
        }
		else
		{
			cuadroTexto.Text = texto.Replace("&", "");
			botonOpcionLabel.BbcodeText = opcion;
			EsconderBotones(opcion);
        }
        GD.Print(opcion);

		animador.Play("CargandoTextoPrincipal");

		if (fondo != "")
		{
			try
			{
                int indice = fondo.LastIndexOf('.');
                fondo = fondo.Substring(0, indice) + ".png";
				fondoSprite.Texture = (Texture)ResourceLoader.Load("res://Images/" + fondo);
            }
            catch (Exception e)
			{
				GD.Print("La imagen no existe");
				GD.Print(e.ToString());
			}
			
		}
	}

	private void ValidarParticion()
	{
		indiceParticion++;

		//Si llegamos al final de la particion reactivamos las opciones
		int valorarParticion = indiceParticion + 1;
        if ((valorarParticion) >= indiceParticionMax)
        {
            //GD.Print("Llegamos al final de la particion");
            //GD.Print(indiceParticion);
            existeParticion = false;
            
            texto = segmentosCargados[indiceParticion];
            ActivarOpciones(2);
        }
		
		if(indiceParticion > 0)
		{
			//GD.Print("mostrando el boton");
			botonVolver.Visible = true;
		}

	}

	private void ParticionarTexto(string rawText)
	{
		List<string> segmentos = new List<string>();
		//GD.Print("quitando visibilidad del boton vovler");
		botonVolver.Visible = false;

		if (rawText.Length < limiteDeCaracteres)
		{
			existeParticion = false;
			texto = rawText;
		}
		else
		{
			try
			{
			ActivarOpciones(0);
			existeParticion = true;
			indiceParticion = 0;
			while (rawText.Length > 0)
			{
				int longitudSegmento = Math.Min(limiteDeCaracteres, rawText.Length);
				string segmento = rawText.Substring(0, longitudSegmento);
				if (longitudSegmento == limiteDeCaracteres)
				{
					int ultimoEspacio = segmento.LastIndexOf('&');
					if (ultimoEspacio < 3 )
					{
						ultimoEspacio = segmento.LastIndexOf(' ');
					}
					if(ultimoEspacio != -1)
					{
						segmento = rawText.Substring(0, ultimoEspacio);
						longitudSegmento = ultimoEspacio;
					}
				}
				segmentos.Add(segmento.Trim());
				rawText = rawText.Substring(longitudSegmento).Trim();
			}

			}
			catch (Exception ex)
			{
				GD.Print(ex.Message);
			}
		}

		segmentosCargados = segmentos;
		
		indiceParticionMax = (segmentosCargados.Count );
	}

    public void GuardarCapitulo(string capitulo)
    {
        var file = new File();

        string path = "user://save.txt";

        file.Open(path, File.ModeFlags.Write);
        file.StoreString(capitulo);
        file.Close();

        GD.Print("Guardado: " + capitulo);
    }
    public string CargarCapitulo()
    {
        var file = new File();
        string path = "user://save.txt";

        if (!file.FileExists(path))
        {
            return "";
	}
        file.Open(path, File.ModeFlags.Read);
        string contenido = file.GetAsText();
        file.Close();

        return contenido;
    }

}