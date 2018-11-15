using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour {

	static public Prospector 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
    public TextAsset layoutXML;
    public float xoffeset = 3;
    public float yoffset = -2.5f;
    public Vector3 layoutCenter;


	[Header("Set Dynamically")]
	public Deck					deck;
    public Layout layout;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;

    void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
        Deck.Shuffle(ref deck.cards);

        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);
	}




    CardProspector Draw()
    {
        CardProspector cd = drawPile[0];
        drawPile.RemoveAt(0);
        return (cd); 
    }

   
    void LayoutGame()
    {
      
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
           
            layoutAnchor = tGO.transform; 
            layoutAnchor.transform.position = layoutCenter;
        }



        CardProspector cp;
      
        foreach (SlotDef tSD in layout.slotDefs)
        {
            
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x,
                layout.multiplier.y * tSD.y, -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            cp.state = eCardState.tableau;
            
            cp.SetSortingLayerName(tSD.layerName); 

            tableau.Add(cp);
        }

        foreach (CardProspector tCP in tableau)
        {
            foreach (int hid in tCP.slotDef.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }

       
        MoveToTarget(Draw());
        
        UpdateDrawPile();
    }

}
