# Typed Scenes version with Addressables
Инструментарий для передачи данных между сценами. Он предоставляет строго-типизированные обёртки для Unity сцен через которые можно комфортно загружать сцены и передавать им данные для работы, использующий загрузку сцен из Addressables.

Плагин разработан на основе плагина школы ЯЮниор - https://ijunior.ru/

## Как установить?

Пакет представляет собой плагин для Unity с открытым исходным кодом. Вам нужно перенести папку Plugins из репозитория в свой Unity проект.

![Plugin Installation](https://i.ibb.co/VvJjQQr/TSI.gif)

После установки у вас могут появиться ошибки.

### Ошибка о том, что наш пакет не может обнаружить System.CodeDom.

Чтобы это исправить вам нужно изменить версию .Net для проекта, по умолчанию стоит 2.0, вам нужно выбрать 4.0.

![Change API Level to 4.0](https://i.ibb.co/9tGZ4h9/TSS.png)

### Ошибка Error CS0234: The type or namespace name 'ResourceManagement' does not exist in the namespace 'UnityEngine' (are you missing an assembly reference?)
Проверьте наличие в Use GUIDs наличие ссылок на 

![enter image description here](https://i.ibb.co/XVv0Tp0/TS.png)

### Проверьте наличие объекта (SceneName)TypedProcessor на сцене.
Для корректной работы на сцене должен находиться объект (SceneName)TypedProcessor.
При его отсутствии сделайте реимпорт сцены или внесите какие-либо изменения и сохраните сцену.
Объект, как и класс создаться автоматически плагином при импорте сцены в проект.

![enter image description here](https://i.ibb.co/WnWBPHp/TPMM.png)

### Addressables
Также все сцены загружаться при помощи Addressables, при их отсутствии в каталоге или не соответствии ключа и имени сцены, будут вызваны соответствующие ошибки при попытках загрузки сцены.

Подробнее про Addressables -https://docs.unity3d.com/Packages/com.unity.addressables@1.15/manual/index.html

После этого всё готово к работе. 

## Как пользоваться?

Пакет самостоятельно генерирует классы-обёртки над сценами в Unity. Вам достаточно добавить сцену в проект и всё произойдёт само. 
Если в вашем проекте уже есть сцены то их достаточно реимпортировать. 

Теперь вы можете запускать сцены через сгенерированные классы. 

```
using UnityEngine;
using IJunior.TypedScenes;

public class Menu : MonoBehaviour
{
    private void OnPlayButtonClick()
    {
        Game.Load();
    }
}
```

## Как передавать данные?

Основная идея этого компонента в том, что у сцены может быть некоторая модель для загрузки/отображения и чтобы запустить сцену нужно ОБЯЗАТЕЛЬНО передать объект в корректном состоянии.

Во-первых, вам нужно задать точку входа в сцене, т.е некоторый код который будет обрабатывать её загрузку. 

```
using IJunior.TypedScenes;
using UnityEngine;

public class StringHandler : MonoBehaviour, ISceneLoadHandler<string>
{
    public void OnSceneLoaded(string argument)
    {
        Debug.Log(argument);
    }
}
```

Для этого вам нужно создать любой компонент реализующий интерфейс ISceneLoadHandler<T>. В качестве T вы указываете те данные которые нужны сцене для запуска.
Наш инструмент сам добавить необходимый метод Load в класс сцены который будет принимать в качестве аргумента подходящие данные. 
После добавления указанного компонента на сцену Game, в класс-сцены появится метод Load(string argument).
После его вызова запустится сцена Game, а у всех компонентов которые реализуют ISceneLoadHandler<string> вызовется метод OnSceneLoaded. 

```
using UnityEngine;
using IJunior.TypedScenes;

public class Menu : MonoBehaviour
{
    private void OnPlayButtonClick()
    {
        Game.Load("Room Name");
    }
}
```
## Как использовать загрузочный экран?

Основная идея этого компонента в том, что у сцены может быть некоторая модель для загрузки/выгрузки сцены с загрузочным экраном.

Во-первых, вам нужно задать точку входа в сцене, т.е некоторый код который будет обрабатывать её загрузку, унаследовав UnloaderLoadingScreen. 
Если при загрузке сцены, на ней будет находиться более чем один объект, с компонентом UnloaderLoadingScreen вызовется ошибка.   

```
using IJunior.TypedScenes;
using UnityEngine;

public class LobbyBuilder : UnloaderLoadingScreen
{
}
```

Наш инструмент сам добавить необходимый метод LoadSceneWithLoadingScreen в класс сцены который будет принимать в качестве аргумента подходящие данные. 
(Вся логика из блока "Как передавать данные?" сохраняет работоспособность.)
После добавления указанного компонента на сцену Game, в класс-сцены появится метод LoadSceneWithLoadingScreen(string argument, string loadingScreenName).
После его вызова запустится сцена Game, и в неё передадутся данные о сцены с загрузочным экраном. 

```
using UnityEngine;
using IJunior.TypedScenes;

public class Menu : MonoBehaviour
{
    private void OnPlayButtonClick()
    {
        Lobby.LoadSceneWithLoadingScreen("T argument", LoadingScene.SceneName);
    }
}
```
Для выгрузки сцены с загрузочным экраном используется метод UnloadLoadingScreen() класса  UnloaderLoadingScreen.
```
using IJunior.TypedScenes;
using UnityEngine;

public class LobbyBuilder : UnloaderLoadingScreen
{
    private MapBuilder _mapBuilder;
    
    private void Awake()
    {
       _mapBuilder.CreateComplited += OnCreateComplited;
       _mapBuilder.Build();    
    }
    
    private void OnCreateComplited()
    {
        _mapBuilder.CreateComplited -= OnCreateComplited;
        UnloadLoadingScreen();
    }
}
```



