import {useAppStore} from "../zustand/useAppStore.js";
import {Alert, Button, Modal, TextInput} from "flowbite-react";
import {useEffect, useRef, useState} from "react";
import {
    getDownloadURL,
    getStorage,
    ref,
    uploadBytesResumable,
} from 'firebase/storage';
import {app} from "../firebase.js";
import {CircularProgressbar} from 'react-circular-progressbar';
import 'react-circular-progressbar/dist/styles.css';
import $axios from "../axios/axios.js";
import {useShallow} from "zustand/react/shallow";
import {HiOutlineExclamationCircle} from "react-icons/hi";


export default function DashProfile() {
    const {
        currentUser,
        errorMessage,
        updateStart,
        updateSuccess,
        updateFailure,
        deleteStart,
        deleteSuccess,
        deleteFailure
    } = useAppStore(useShallow((state) => ({
        currentUser: state.currentUser,
        errorMessage:state.errorMessage,
        updateStart: state.updateStart,
        updateSuccess: state.updateSuccess,
        updateFailure: state.updateFailure,
        deleteStart: state.deleteStart,
        deleteSuccess: state.deleteSuccess,
        deleteFailure: state.deleteFailure
    })));

    const [imageFile, setImageFile] = useState(null);
    const [imageFileUrl, setImageFileUrl] = useState(null);
    const filePickerRef = useRef();
    const [imageFileUploadProgress, setImageFileUploadProgress] = useState(null);
    const [imageFileUploadError, setImageFileUploadError] = useState(null);
    const [formData, setFormData] = useState({});
    const [imageFileUploading, setImageFileUploading] = useState(false);
    const [updateUserSuccess, setUpdateUserSuccess] = useState(null);
    const [updateUserError, setUpdateUserError] = useState(null);
    const [showModal, setShowModal] = useState(false);


    const handleImageChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setImageFile(file);
            setImageFileUrl(URL.createObjectURL(file));
        }
    }

    useEffect(() => {
        if (imageFile) {
            uploadImage();
        }
    }, [imageFile]);

    const uploadImage = async () => {
        setImageFileUploading(true);
        setImageFileUploadError(null);
        const storage = getStorage(app);
        const fileName = new Date().getTime() + imageFile.name;
        const storageRef = ref(storage, fileName);
        const uploadTask = uploadBytesResumable(storageRef, imageFile);
        uploadTask.on(
            'state_changed',
            (snapshot) => {
                const progress =
                    (snapshot.bytesTransferred / snapshot.totalBytes) * 100;
                setImageFileUploadProgress(progress.toFixed(0));
            },
            (error) => {
                setImageFileUploadError("Could not upload image (File must be less than 2MB)");
                setImageFileUploadProgress(null);
                setImageFile(null);
                setImageFileUrl(null);
                setImageFileUploading(false);
            },
            () => {
                getDownloadURL(uploadTask.snapshot.ref).then((downloadURL) => {
                    setImageFileUrl(downloadURL);
                    setFormData({...formData, profilePictureUrl: downloadURL});
                    setImageFileUploading(false);
                });
            }
        );
    };

    const handleChange = (e) => {
        setFormData({...formData, [e.target.id]: e.target.value});
    }

    const handleSubmit = async (e) => {
        e.preventDefault();

        setUpdateUserError(null);
        setUpdateUserSuccess(null);

        if (Object.keys(formData).length === 0) {
            setUpdateUserError("No changes made");
            return;
        }

        if (imageFileUploading) {
            setUpdateUserError("Please wait for image to upload");
            return;
        }

        try {
            updateStart();

            const {data} = await $axios.put(`user/update/${currentUser.id}`, formData);

            if (!data.isSucceed) {
                updateFailure(data.errorCode);
                setUpdateUserError(data.errorCode);
            } else {
                updateSuccess(data.payload);
                setUpdateUserSuccess("User's profile updated successfully");
            }
        } catch (error) {
            if (error.response) {
                updateFailure(error.response.data.payload.detail);
                setUpdateUserError(error.response.data.errorCode);
                return;
            }

            updateFailure(error.message);
            setUpdateUserError("The server is not responding. Please try again later.");
        }
    }

    const handleDeleteUser = async () => {
        setShowModal(false);

        try {
            deleteStart();
            const {data} = await $axios.delete(`user/delete/${currentUser.id}`);

            if(!data.isSucceed) {
                deleteFailure(data.errorCode);
            } else {
                deleteSuccess();
            }
        } catch (error) {
            const {errorCode} = error.response.data;
            if (errorCode) {
                deleteFailure(errorCode);
            } else {
                deleteFailure(error.message);
            }
        }
    };

    return (
        <div className="max-w-lg mx-auto p-3 w-full">
            <h1 className="my-7 text-center font-semibold text-3xl">Profile</h1>
            <form
                onSubmit={handleSubmit}
                className="flex flex-col gap-4">
                <input
                    type="file"
                    accept="image/*"
                    onChange={handleImageChange}
                    ref={filePickerRef}
                    hidden
                />
                <div
                    className="relative w-32 h-32 self-center cursor-pointer shadow-md overflow-hidden rounded-full"
                    onClick={() => filePickerRef.current.click()}
                >
                    {imageFileUploadProgress && (
                        <CircularProgressbar
                            value={imageFileUploadProgress || 0}
                            text={`${imageFileUploadProgress}%`}
                            strokeWidth={5}
                            styles={{
                                root: {
                                    width: '100%',
                                    height: '100%',
                                    position: 'absolute',
                                    top: 0,
                                    left: 0,
                                },
                                path: {
                                    stroke: `rgba(62, 152, 199, ${
                                        imageFileUploadProgress / 100
                                    })`,
                                },
                            }}
                        />
                    )}
                    <img
                        src={imageFileUrl || currentUser.profilePictureUrl}
                        alt="Profile Picture"
                        className={`rounded-full w-full h-full object-cover border-8 border-[lightgray] ${
                            imageFileUploadProgress &&
                            imageFileUploadProgress < 100 &&
                            'opacity-60'
                        }`}
                    />
                </div>
                {imageFileUploadError && (
                    <Alert color="failure">{imageFileUploadError}</Alert>
                )}

                <TextInput
                    type="text"
                    id="username"
                    placeholder="username"
                    defaultValue={currentUser.username}
                    onChange={handleChange}
                />
                <TextInput
                    type="email"
                    id="email"
                    placeholder="email"
                    defaultValue={currentUser.email}
                    onChange={handleChange}
                />
                <TextInput
                    type="password"
                    id="currentPassword"
                    placeholder="current password"
                    onChange={handleChange}
                />
                <TextInput
                    type="password"
                    id="newPassword"
                    placeholder="new password"
                    onChange={handleChange}
                />
                <Button
                    type="submit"
                    gradientDuoTone="purpleToBlue"
                    outline

                >
                    Update
                </Button>
                <div className="text-red-500 flex justify-between mt-5">
                    <span
                        className="cursor-pointer"
                        onClick={() => setShowModal(true)}
                    >
                        Delete Account
                    </span>
                    <span className="cursor-pointer">Sign Out</span>
                </div>
                {updateUserSuccess &&
                    <Alert
                        color="success"
                        className="mt-5"
                    >
                        {updateUserSuccess}
                    </Alert>
                }
                {updateUserError &&
                    <Alert
                        color="failure"
                        className="mt-5"
                    >
                        {updateUserError}
                    </Alert>
                }
                {errorMessage &&
                    <Alert
                        color="failure"
                        className="mt-5"
                    >
                        {errorMessage}
                    </Alert>
                }
                <Modal
                    show={showModal}
                    onClose={() => setShowModal(false)}
                    popup
                    size="md"
                >
                    <Modal.Header/>
                    <Modal.Body>
                        <div className="text-center">
                            <HiOutlineExclamationCircle
                                className="h-14 w-14 text-gray-400 dark:text-gray-200
                            mb-4 mx-auto"
                            />
                            <h3
                                className="mb-5 text-lg text-gray-500 dark:text-gray-400"
                            >
                                Are you sure you want to delete your account?
                            </h3>
                            <div className="flex justify-center gap-4">
                                <Button
                                    color="failure"
                                    onClick={handleDeleteUser}
                                >
                                    Yes, I&apos;m sure
                                </Button>
                                <Button
                                    color="gray"
                                    onClick={() => setShowModal(false)}
                                >
                                    No, cancel
                                </Button>
                            </div>
                        </div>
                    </Modal.Body>
                </Modal>
            </form>
        </div>
    );
}