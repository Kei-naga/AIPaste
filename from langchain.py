from langchain.prompts import ChatPromptTemplate
from langchain_core.pydantic_v1 import BaseModel, Field
from langchain_core.output_parsers import PydanticOutputParser
import json

from typing import List
from pprint import pprint

from langchain_nvidia_ai_endpoints import ChatNVIDIA
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import JsonOutputParser, StrOutputParser
from langchain_core.runnables import RunnableLambda
from langchain_core.pydantic_v1 import BaseModel, Field

from assessment_helper import run_assessment

from langchain.chains import TransformChain
from collections import Counter

base_url = 'http://llama:8000/v1'
model = 'meta/llama-3.1-8b-instruct'
llm = ChatNVIDIA(base_url=base_url, model=model, temperature=0)

with open('data/emails.json', 'r') as f:
    emails = json.load(f)

class EmailAnalysis(BaseModel):
    """Combined details of the email"""
    product_category: str = Field(description="Specific product type")
    is_negative: bool = Field(description="Negative sentiment flag")
    store_location: str = Field(description="Store location")

analysis_parser = JsonOutputParser(pydantic_object=EmailAnalysis)
format_instructions = analysis_parser.get_format_instructions()

template = ChatPromptTemplate.from_messages([
    ("system", "You are an AI that generates JSON and only JSON according to the instructions provided to you."),
    ("human", (
        "Generate JSON about the user input according to the provided format instructions.\n" +
        "Input: {email}\n" +
        "Format instructions {format_instructions}")
    )
])
template_with_format_instructions = template.partial(format_instructions=format_instructions)
processing_chain = template_with_format_instructions | llm | analysis_parser

def aggregate_analyses(inputs):
    negative_categories = Counter()
    complaint_locations = Counter()
    
    for analysis in inputs["analyses"]:
        if analysis["is_negative"]:
            negative_categories[analysis["product_category"]] += 1
            complaint_locations[analysis["store_location"]] += 1
            
    return {
        "result": f"""The product category with the most negative sentiment is {
            negative_categories.most_common(1)[0][0]
        }.\n\nThe store location with the most negative sentiment is {
            complaint_locations.most_common(1)[0][0]
        }."""
    }

def process_emails(emails):
    analyses = [processing_chain.invoke({"email": email}) for email in emails]
    return aggregate_analyses({"analyses": analyses})["result"]

full_chain = RunnableLambda(process_emails)
full_chain.invoke(emails)