# 🧠 Trivia Game - Unity + C# Case Study

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)

A mobile-first Trivia Game developed in Unity using C#. This project was created as a case study for a mobile game, focusing on **Object-Oriented Programming (OOP) principles, Design Patterns, and UI Optimization**.

## ✨ Features

*   **Dynamic Quiz Flow:** Built with the **State Pattern** to seamlessly transition between gameplay states (Waiting For Answer, Showing Result, and Finished).
*   **Configurable Settings:** Uses `ScriptableObjects` (`GameConfig`) to easily tweak score rewards/penalties, API URLs, and timer durations without modifying code.
*   **API Integration:** Fetches quiz questions and paginated leaderboard data asynchronously using `UnityWebRequest` and custom JSON serialization.
*   **Optimized Paginated Leaderboard:** Implements **Object Pooling** and dynamic index calculations within a `ScrollRect` to handle large amounts of leaderboard data efficiently without frame drops.
*   **Memory Management:** Uses Unity **Addressables** for asynchronous instantiation and memory release of UI screens (Main Menu, Gameplay, Leaderboard, Game Over).
*   **Event-Driven UI:** Decoupled UI logic using C# Events (`Action`) for updating the score with smooth lerp animations and filling the timer slider dynamically.

## 📥 Download & Play (Releases)

You can easily test the game on your Android device without building it from the source code. A pre-compiled mobile build is available in the [Releases](../../releases) section:
* **Android (.apk):** Download the `.apk` file and install it directly on your Android phone or emulator.

## 🏗️ Architecture & Design Patterns

*   **State Pattern:** `IQuizState` handles the gameplay loop.
*   **Singleton Pattern:** `GameManager` acts as the global entry point for UI flow and scene management.
*   **Object Pooling:** Used heavily in the `LeaderBoardPopUp` to recycle list items as the user scrolls, drastically improving UI performance.
*   **Observer Pattern:** `Timer` and `QuizManager` trigger events that `ScoreView` and `TimerSliderView` listen to, keeping logic and presentation separated.
*   **Interface Segregation:** `IApiService` abstracts the network calls, making the system testable and modular.

## 🚀 Getting Started

### Prerequisites
*   **Unity** (2022.3 LTS or newer recommended)
*   **Addressables Package** (Installed via Package Manager)
*   **TextMeshPro**

### Installation
1. Clone the repository.
2. Open the project in Unity.
3. Ensure that the **Addressables** groups are built and configured. (The `Main Menu`, `Leaderboard`, `Game Play`, and `Game Over` prefabs must be marked as Addressable).
4. Set the build target to **Android** in Build Settings to preview the portrait layout correctly.
5. Open the startup scene and hit Play!

## 🎮 Gameplay Details
*   **Scoring System:** 
    *   Correct Answer: +10 points
    *   Wrong Answer: -5 points
    *   Timeout: -3 points
*   **Time Limit:** 20 seconds per question.
*   Visual feedback highlights selected answers in Green (Correct) or Red (Wrong) before smoothly transitioning to the next question.

## 📄 Credits
Developed as part of the Mage Games Case Study During Internship.
