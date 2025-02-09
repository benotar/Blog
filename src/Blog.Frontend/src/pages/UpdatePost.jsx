import {useEffect, useState} from "react";
import {useNavigate, useParams} from "react-router-dom";
import $axios from "../axios/axios.js";
import {getDownloadURL, getStorage, ref, uploadBytesResumable} from "firebase/storage";
import {app} from "../firebase.js";
import {Alert, Button, FileInput, Select, TextInput} from "flowbite-react";
import {CircularProgressbar} from "react-circular-progressbar";
import ReactQuill from "react-quill";
import {useAppStore} from "../zustand/useAppStore.js";

const UpdatePost = () => {

    const currentUser = useAppStore((state) => state.currentUser);
    const [file, setFile] = useState(null);
    const [categories, setCategories] = useState([]);
    const [imageUploadProgress, setImageUploadProgress] = useState(null);
    const [imageUploadError, setImageUploadError] = useState(null);
    const [formData, setFormData] = useState({});
    const [updateError, setUpdateError] = useState(null);
    const {postId} = useParams();

    const navigate = useNavigate();

    useEffect(() => {
        const fetchPost = async () => {
            try {
                const {data} = await $axios.get(`post/get-posts?postId=${postId}`);

                if (!data.isSucceed) {
                    console.log(data.errorCode);
                    setUpdateError(data.errorCode);
                    return;
                }

                setUpdateError(null);
                setFormData(data.payload.data.items[0]);
            } catch (error) {
                console.log(error.message);
            }
        }
        fetchPost();
    }, [postId]);

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

    const handleUploadImage = async () => {
        try {
            if (!file) {
                setImageUploadError("Please select an image");
                return;
            }
            setImageUploadError(null);
            const storage = getStorage(app);
            const fileName = new Date().getTime() + '-' + file.name;
            const storageRef = ref(storage, fileName);
            const uploadTask = uploadBytesResumable(storageRef, file);
            uploadTask.on(
                "state_changed",
                (snapshot) => {
                    const progress = (snapshot.bytesTransferred / snapshot.totalBytes) * 100;
                    setImageUploadProgress(progress.toFixed(0));
                },
                (error) => {
                    setImageUploadError("Could not upload image (File must be less than 2MB)");
                    setFormData({...formData, "imageUrl": null});
                    setImageUploadProgress(null);
                    console.log(error);
                },
                () => {
                    getDownloadURL(uploadTask.snapshot.ref).then((downloadURL) => {
                        setImageUploadProgress(null);
                        setImageUploadError(null);
                        setFormData({...formData, "imageUrl": downloadURL});
                    });
                }
            );
        } catch (error) {
            setImageUploadError("Image upload failed");
            setImageUploadProgress(null);
            console.log(error);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const {data} = await $axios.put(`post/update-post/${formData.id}/${currentUser.id}`, formData);

            if (!data.isSucceed) {
                setUpdateError(data.errorCode);
                return;
            }
            setUpdateError(null);
            navigate(`/post/${data.payload.slug}`);
        } catch (error) {
            console.error(error);
            setUpdateError("Something went wrong");
        }
    };


    console.log(formData)

    return (
        <div className="p-3 max-w-3xl mx-auto min-h-screen">
            <h1
                className="text-center text-3xl my-7 font-semibold"
            >
                Update post
            </h1>
            <form
                className="flex flex-col gap-4"
                onSubmit={handleSubmit}
            >
                <div className="flex flex-col gap-4 sm:flex-row justify-between">
                    <TextInput
                        type="text"
                        placeholder="Title"
                        value={formData?.title}
                        required
                        id="title"
                        className="flex-1"
                        onChange={(e) =>
                            setFormData({...formData, [e.target.id]: e.target.value})
                        }
                    />
                    <Select
                        id="category"
                        value={formData.category ? formData.category.charAt(0).toLowerCase() + formData.category.slice(1) : ""}
                        onChange={(e) =>
                            setFormData({...formData, [e.target.id]: e.target.value})
                        }
                    >
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
                        onChange={(e) => setFile(e.target.files[0])}
                    />
                    <Button
                        type="button"
                        gradientDuoTone="purpleToBlue"
                        size="sm"
                        outline
                        onClick={handleUploadImage}
                        disabled={imageUploadProgress}
                    >
                        {
                            imageUploadProgress ? (
                                <div className="w-16 h-16">
                                    <CircularProgressbar
                                        value={imageUploadProgress}
                                        text={`${imageUploadProgress || 0}%`}
                                    />
                                </div>
                            ) : (
                                "Upload Image"
                            )
                        }
                    </Button>
                </div>
                {
                    imageUploadError && (
                        <Alert color="failure">
                            {imageUploadError}
                        </Alert>
                    )
                }
                {
                    formData.imageUrl && (
                        <img
                            src={formData.imageUrl}
                            alt="upload"
                            className="w-full h-72 object-cover"
                        />
                    )
                }
                <ReactQuill
                    theme="snow"
                    placeholder="Write something"
                    className="h-72 mb-12"
                    required
                    onChange={
                        (value) => {
                            setFormData({...formData, content: value});
                        }
                    }
                    value={formData?.content || ""}
                />
                <Button
                    type="submit"
                    gradientDuoTone="purpleToPink"
                >
                    Update post
                </Button>
                {
                    updateError && <Alert
                        color="failure"
                        className="mt-5"
                    >
                        {updateError}
                    </Alert>
                }
            </form>
        </div>
    );
}


export default UpdatePost;