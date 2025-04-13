# 📘 Get Started 

This document describes how the `get-started.md` page was generated and the rationale behind its structure. It is intended for contributors and documentation maintainers to understand the decisions and process behind the onboarding content.

---

## ✅ Purpose

The **"Get Started"** page serves as an onboarding guide for developers using the Unity project template. It helps them clone the repository, initialize submodules, and open the project in Unity.

---

## 🧭 Guidance Used to Generate the Page

### 1. **GitHub Template Repository**

The template is based on this repository:

> [https://github.com/felixwongong/cfUnityTemplate](https://github.com/felixwongong/cfUnityTemplate)

This is the primary entry point for users to get started.

---

### 2. **Clone and Submodule Initialization**

Since the repository uses Git submodules (`cfEngine`, `cfUnityEngine`), the instructions include:

#### ✅ Primary clone with submodules:

```bash
git clone --recurse-submodules https://github.com/felixwongong/cfUnityTemplate.git
```

An additional fallback command was also included for users who cloned the repository without submodules:

```bash
git submodule update --init --recursive
```
