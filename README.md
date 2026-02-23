# OpenCode Installation and Usage Guide ( Free Model + Antigravity )
## Overview

OpenCode is an open-source AI coding agent that can:

* Understand your codebase
* Execute coding tasks automatically
* Use plugins and tools
* Connect to free, local, or paid models

This guide walks you through installing and using OpenCode step-by-step.

---
![Open Code demo](./doc/opencode-demo.gif)
---

# Step 1: Install Prerequisites

## 1. Install Node.js

Download and install:

https://nodejs.org/

Verify:

```bash
node -v
npm -v
```

---

## 2. Install Git

Download:

https://git-scm.com/

Verify:

```bash
git --version
```

---

## 3. (Optional but Recommended) Install Ollama for Free Local Models

Download:

https://ollama.com/

Verify:

```bash
ollama --version
```

Pull a model:

```bash
ollama pull llama3
```

---

# Step 2: Install OpenCode

Clone repository:

```bash
git clone https://github.com/opencode-ai/opencode.git

# install opencode
npm i -g opencode-ai
```

Go to folder:

```bash
cd opencode
```

Install dependencies:

```bash
npm install
```

---

# Step 3: Configure AI Model

Open config file:

```bash
config.json
```

Example using Ollama:

```json
{
  "provider": "ollama",
  "model": "llama3"
}
```

Example using API:

```json
{
  "provider": "openai",
  "model": "gpt-4"
}
```

---

# Step 4: Start OpenCode

Run:

```bash
npm run start
```

You should see:

```
OpenCode started successfully
```

---

# Step 5: Open Your Project

Navigate to your coding project:

Example:

```bash
cd my-project
```

Start OpenCode inside project:

```bash
opencode
```

---

# Step 6: Give Your First Task

Example prompts:

```
Explain this project
```

```
Fix errors
```

```
Add login feature
```

```
Refactor code
```

OpenCode will:

* Read files
* Analyze code
* Modify code
* Execute commands

---

# Step 7: Enable Plugins

Plugins allow OpenCode to:

* Read files
* Write files
* Execute terminal commands
* Browse internet

Example plugin config:

```json
{
  "plugins": [
    "filesystem",
    "terminal",
    "web"
  ]
}
```

---

# Step 8: Example Workflow

Example:

Prompt:

```
Create REST API using Node.js
```

OpenCode will:

Step 1: Plan
Step 2: Create files
Step 3: Write code
Step 4: Install dependencies
Step 5: Run project

Automatically.

---

# Step 9: Use Free Models (Save Money)

Recommended free models:

Using Ollama:

```
llama3
mistral
codellama
deepseek-coder
```

Run:

```bash
ollama pull codellama
```

---

# Step 10: Best Practices

Recommended setup:

* Use Ollama (free)
* Enable plugins
* Run inside project folder
* Give clear instructions

Example:

Good:

```
Create Sample Weather .NET Web API with JWT authentication
```

Bad:

```
Fix code
```

---

# Step 11: Real-World Use Cases

OpenCode can:

* Build full applications
* Fix bugs
* Refactor code
* Write tests
* Explain code
* Automate development

---

# Step 12: Advantages of OpenCode

Advantages:

* Free
* Open-source
* No subscription
* Works with local models
* Fully customizable

---

# Conclusion

You now have fully working OpenCode.

You can use it as your personal AI software engineer.

---

# Recommended Setup (Best Combination)

Recommended stack:

* OpenCode
* Ollama
* deepseek-coder model

This gives:

* Free
* Fast
* Powerful

---

# Antigravity + Gemini CLI OAuth Plugin for Opencode

## Option A: Let an LLM do it

```
# Paste this into any LLM agent (Claude Code, OpenCode, Cursor, etc.):
Install the opencode-antigravity-auth plugin and add the Antigravity model definitions to ~/.config/opencode/opencode.json by following: https://raw.githubusercontent.com/NoeFabris/opencode-antigravity-auth/dev/README.md
```

# Login with your Google account:
```
opencode auth login

```
# End of Guide
