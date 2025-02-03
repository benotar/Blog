import {Button, FileInput, Select, TextInput} from "flowbite-react";
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import {useEffect, useState} from "react";
import $axios from "../axios/axios.js";

const CreatePost = () => {

    const [categories, setCategories] = useState([]);

    useEffect(() => {
        const getCategories = async () => {
            try {
                const {data} = await $axios.get("post/get-categories");

                if (data) {
                    setCategories(data.payload);
                }
            } catch (error) {
                console.error("Error fetching categories:", error);
            }
        }

        getCategories();
    }, []);

    console.log(categories);

    return (
        <div className="p-3 max-w-3xl mx-auto min-h-screen">
            <h1
                className="text-center text-3xl my-7 font-semibold"
            >
                Create a post
            </h1>
            <form
                className="flex flex-col gap-4"
            >
                <div className="flex flex-col gap-4 sm:flex-row justify-between">
                    <TextInput
                        type="text"
                        placeholder="Title"
                        required
                        id="title"
                        className="flex-1"
                    />
                    <Select>
                        {
                            categories.map(category => {
                                let categoryValue = category.charAt(0).toLowerCase() + category.slice(1);

                                return (
                                    <option key={categoryValue} value={categoryValue}>
                                        {
                                            categoryValue === "uncategorized"
                                                ? "Select a category"
                                                : categoryValue === "dotnet"
                                                    ? "C#/.NET"
                                                    : category
                                        }
                                    </option>
                                );
                            })
                        }
                    </Select>
                </div>
                <div
                    className="flex gap-4 items-center justify-between border-4 border-teal-500
                    border-dotted p-3"
                >
                    <FileInput
                        type="file"
                        accept="image/*"
                    />
                    <Button
                        type="button"
                        gradientDuoTone="purpleToBlue"
                        size="sm"
                        outline
                    >
                        Upload image
                    </Button>
                </div>
                <ReactQuill
                    theme="snow"
                    placeholder="Write something"
                    className="h-72 mb-12"
                    required
                />
                <Button
                    type="submit"
                    gradientDuoTone="purpleToPink"
                >
                    Publish
                </Button>
            </form>
        </div>
    );
};

export default CreatePost;