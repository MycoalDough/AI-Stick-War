# 🤖 AI Stick War

**The classic Stick War—played by Reinforcement Learning AIs!**

<p align="center">
  <img src="https://img.shields.io/github/stars/MycoalDough/AI-Stick-War?style=social" alt="GitHub stars">
  <img src="https://img.shields.io/github/forks/MycoalDough/AI-Stick-War?style=social" alt="GitHub forks">
  <img src="https://img.shields.io/github/languages/top/MycoalDough/AI-Stick-War" alt="Top language">
</p>

---

## 🕹️ What is this?

A full recreation of Stick War 2’s gameplay environment in Unity… but *all the players are RL agents*.  
Agents learn to play both sides—ORDER and CHAOS—using advanced Deep Reinforcement Learning, all controlled in real time through socket connections.

> "Stick War was probably THE game of my life (that and Cartoon Wars). After learning how to program RL, I *had* to see if I could make AIs play this. Now they're so strong even I can't beat them! 🥲"

---

## ✨ Features

- **Unity-powered simulation**  
- AI agents replicate both [ORDER] and [CHAOS] factions
- State-of-the-art RL: Dueling Double Q-Networks, Prioritized Replay, N-Step Returns, Noisy Nets
- Multi-threaded sockets: RL model and environment run asynchronously for speed
- Near-complete cover of Stick War 2 mechanics (except a few upgrades, cut for faster iteration)
- All project files included—open source, hackable!

---

## 👾 How it Works

- The game runs in Unity, with full physics, visuals, and custom controls
- [Socket]-based connection lets RL agents communicate with the environment:
  - Agents get observations, send their next action—repeat!
- Two agents play at once: one as **ORDER** (the knights), the other as **CHAOS** (the monsters)
- **Dueling Double Q-Learning** drives the learning for both
- Training is *slow* but powerful: targeting 30k+ episodes for strong play

---

## 📈 Progress

- Training currently running at ~2k iterations (as of 5/18/24)
- Goal: 30k+ iterations for polished high-level AI on both teams
- Even now, after training, the agents are tough—*I can barely win against them anymore!*

---

## 📂 What's in this Repo?

- **Unity editor project** — full source, prefabs, scripts, scenes
- **AI scripts** (Python) for RL agents: model, training loop, environment wrappers
- [Socket] code to bridge Unity simulation and agent training
- All assets, code, configs! Fork and study away

---

## 📸 Screenshots

<p align="center">
  <img src="https://i.imgur.com/n4W7F1b.png" alt="Stick War AI agents fighting">
</p>
<p align="center">
  <img src="https://i.imgur.com/Wp8pPZ4.png" alt="Unity project with agent controls">
</p>

<sub>*(Replace the image links with actual screenshots from your project for more awesomeness!)*</sub>

---

## 🏗️ Tech Stack

- **Unity** (C#): Simulation and environment editor
- **Python**: RL agent logic (Double DQN, Prioritized Replay, Noisy Nets, N-Step...)
- **Sockets & Threads**: Fast, real-time agent-environment communication

---

## 📜 License

MIT License — use, fork, remix!

---

## 🙏 Credits

- **Stick War** (original): Max Games  
  (This is a non-commercial fan project)
- **Deep RL research:** open-source RL community + papers

---

<p align="center">
  <b>Built by MycoalDough, powered by nostalgia and neural networks 🤖⚔️</b>
</p>
