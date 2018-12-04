using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UILevelSelect : MonoBehaviour {
	[SerializeField] private LevelController levelController;
	[SerializeField] private UILevel levelUI;
	[SerializeField] LevelPopup levelPopup;

	private Transform levelSelectPanel;
	private int currentPage;
	private List<UILevel> levelList = new List<UILevel>();
	private int maxPage;




	// Use this for initialization
	void Start () {
		levelSelectPanel = transform;
		levelPopup.gameObject.SetActive(false);
		for(int i = 0 ; i < levelController.levels.Count ; i++ ){
			levelList.Add(levelUI);
		}
		BuildLevelPage(LevelDataInfo.lastPage);	
	}
	
	void BuildLevelPage(int page){
		RemoveAllUILevel();
		currentPage = page;
		int pageSize = 12;
		List<UILevel> pageLists = levelList.Skip(page * pageSize).Take(pageSize).ToList();
		maxPage = Mathf.CeilToInt(levelList.Count/12);

		for(int i = 0 ; i < pageLists.Count ; i++){
			Level level = levelController.levels[(pageSize * page) +i];
			UILevel uiLevel = Instantiate(pageLists[i]);
			uiLevel.SetStars(level.Stars);
			Debug.LogFormat("level.Stars{0}",level.Stars);
			uiLevel.transform.SetParent(levelSelectPanel);
			uiLevel.GetComponent<Button>().onClick.AddListener(() => SelectLevel(level));

			if(!level.Locked){
				uiLevel.lockImage.SetActive(false);
				uiLevel.LevelIDText.text = level.ID.ToString();
				uiLevel.LevelNameText.text = level.LevelName.ToString();
			}else{
				uiLevel.lockImage.SetActive(true);
				uiLevel.LevelIDText.text = "";
			}
		}

	}


	void RemoveAllUILevel(){
		for(int i = 0 ; i < levelSelectPanel.childCount ; i++){
			Destroy(levelSelectPanel.GetChild(i).gameObject);
		}
	}

	public void NextPage(){
		if(currentPage < maxPage){
			LevelDataInfo.lastPage = currentPage+1;
			BuildLevelPage(currentPage+1);	
		}
	}

	public void PrePage(){
		if(currentPage >0){
			LevelDataInfo.lastPage = currentPage-1;
			BuildLevelPage(currentPage-1);	
		}
	}

	void SelectLevel(Level level)
	{
		if(level.Locked){
			levelPopup.gameObject.SetActive(true);
			levelPopup.setText("<b>Level"+level.ID+"is unlock</b>\nComplete "+(level.ID-1)+" first.");
		}else{
			levelController.LevelStart(level.LevelName);
		}
	}


}
