# Dairy-Service Architecture

This document visualizes the strict 4-Tier Architecture of the Dairy-Service project, focusing on the restaurant analogy and the system context.

## 1. The Architecture (Visualized by Analogy)

Here is a visual representation of the architecture using the Restaurant analogy described in the `Readme.md`:

![Restaurant Architecture Analogy](file:///C:/Users/hp/.gemini/antigravity-ide/brain/4d463ae6-a4d2-47ed-ac5c-9a980fff0634/architecture_restaurant_analogy_1784285087602.png)

* **The Customer (Frontend):** Makes the request via the browser.
* **The Waiter (ServiceHub API):** Takes the request and acts as the entry point.
* **The Plated Meal (DTO):** The safely formatted data sent back to the customer.
* **The Chef (Context/DAL):** The only layer allowed to talk to the database.
* **Raw Ingredients (DMO):** The exact shape of the data in the database.
* **The Pantry (MongoDB):** The storage database.

---

## 2. Code Relationships (Folder Structure)

Here is a diagram representing the exact code relationships between the folders as defined in your Readme:

![Folder Relationships Flowchart](file:///C:/Users/hp/.gemini/antigravity-ide/brain/4d463ae6-a4d2-47ed-ac5c-9a980fff0634/folder_relationships_diagram_1784286264082.png)
