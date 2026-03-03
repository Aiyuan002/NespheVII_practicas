using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class InkTestingScript : MonoBehaviour
{
    public TextAsset inkJSON;
    private Story story;
    private string knot;
    private string key;
    private int currentIndex;
    private string currentKnot = "";

    public Text textPrefab;
    public Button buttonPrefab;

    public Button nextButton;

    // Start is called before the first frame update
    void Start()
    {
        story = new Story(inkJSON.text);
        nextButton.onClick.AddListener(() => RefreshUI());

        RefreshUI(); 
    }

    void RefreshUI()
    {
        EraseUI();

        //Cargar texto en el elemento Text
        Text storyText = Instantiate(textPrefab) as Text;
        storyText.text = loadStoryChunk();
        storyText.transform.SetParent(this.transform, false);

        //Cargar opciones en botones 
        foreach (Choice choice in story.currentChoices)
        {
            //Cargar texto en los elementos Button
            Button choiceButton = Instantiate(buttonPrefab) as Button;
            Text choiceText = choiceButton.GetComponentInChildren<Text>();
            choiceText.text = choice.text;

            //Traducir texto botones
            currentIndex++;
            key = currentKnot + "_" + currentIndex;
            choiceText.text = LocalisationSystem.GetLocalisedValues(key)?.Trim();

            choiceButton.transform.SetParent(this.transform, false);

            choiceButton.onClick.AddListener(delegate { ChooseStoryChoice(choice); });  
        }
    }

    //Cargar texto de la opción elegida
    void ChooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshUI();
    }

    string loadStoryChunk()
    {
        string text = "";
        string locText = "";

        if (story.canContinue)
        {
            //Texto del fichero Ink
            text = story.Continue();
            text = text?.Trim();

            //Recuperar el nudo actual
            //knot = story.state.currentPathString?.Split('.')[0] ?? _currentKnot;

            if(story.currentTags.Count > 0)
            {
                knot = story.currentTags[0];
            }
            

            //Comprobar si ha cambiado de nudo
            if (currentKnot != knot)
            {
                currentKnot = knot;
                currentIndex = 1;
            }
            else
            {
                currentIndex++;
            }

            key = currentKnot + "_" + currentIndex;

            //Texto traducido
            locText = LocalisationSystem.GetLocalisedValues(key);
            locText = locText?.Trim();
        }

        return locText;
    }

    //Limpiar pantalla
    void EraseUI()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }
}
