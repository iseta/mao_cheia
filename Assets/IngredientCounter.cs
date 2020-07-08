using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientCounter : MonoBehaviour
{
    public GameObject[] ingredientsRequired;
    public int[] quantidadeIngredientes;
    public Object prefabIngredienteUI;
    public int countWin;
    private bool winCondition = false;
    public Text[] txtCounter;
    public Animator animIntro, animOutro;
    public Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.FindObjectOfType<Board>();
        countWin = ingredientsRequired.Length;
        for (int i = 0; i<ingredientsRequired.Length; i++)
        {
            GameObject g = Instantiate(prefabIngredienteUI, transform) as GameObject;
            g.GetComponentInChildren<Image>().color = ingredientsRequired[i].GetComponent<SpriteRenderer>().color;
            g.GetComponentInChildren<Text>().text = "= " + quantidadeIngredientes[i].ToString();
            txtCounter[i] = g.GetComponentInChildren<Text>();
        }
    }

    public void decreaseIngredient(GameObject go)
    {
        for(int i = 0; i < ingredientsRequired.Length; i++)
        {
            if (ingredientsRequired[i].tag.Equals(go.tag) && quantidadeIngredientes[i] > 0)
            {
                quantidadeIngredientes[i]--;
                if (quantidadeIngredientes[i] <= 0)
                {
                    countWin--;
                }
            }

            txtCounter[i].text = "= " + quantidadeIngredientes[i].ToString();
        }
        if (countWin == 0)
        {
            winCondition = true;
        }
    }

    private void Update()
    {
        if(winCondition && board.currentState == GameState.move)
        {
            animIntro.Play("anim_slide_left_enter");
            animOutro.Play("anim_slide_left_exit");
        }
    }

    public void closeGame()
    {
        Application.Quit();
    }
}
