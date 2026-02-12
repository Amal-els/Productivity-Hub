from fastapi import FastAPI
from pydantic import BaseModel
from openai import OpenAI
import os

app = FastAPI()

client = OpenAI(
    api_key=os.getenv("OPENAI_API_KEY"),
    base_url="https://openrouter.ai/api/v1"
)

# Request body model
class ChatRequest(BaseModel):
    message: str

BASE_MESSAGES = [
    {
        "role": "system",
        "content": (
            "You are a productivity assistant. "
            "You help users manage tasks, notes, focus and time. "
            "Be concise and practical."
            "Your answers are short and straight to the point. "
            "You have to summmarize long answers."
            "Be friendly and welcoming!"
            " Avoid symbols like # and *"
            "Use bullet points, headings, and numbering when necessary. "
            "Do NOT output plain text blocks. "
            "Always format for readability in a web app."
        )
    }
]

@app.post("/chat")
def chat(request: ChatRequest):

    messages = BASE_MESSAGES + [
        {"role": "user", "content": request.message}
    ]

    response = client.chat.completions.create(
        model="mistralai/mistral-7b-instruct",
        messages=messages,
        temperature=0.3
    )

    return {
        "reply": response.choices[0].message.content
    }
