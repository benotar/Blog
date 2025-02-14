import moment from "moment";
import {FaThumbsUp} from "react-icons/fa";
import {useAppStore} from "../zustand/useAppStore.js";
import {ADMIN} from "../shared/utils.js";
import {useState} from "react";
import {Button, Textarea} from "flowbite-react";
import $axios from "../axios/axios.js";

const Comment = ({
                     comment,
                     onLike,
                     onEdit,
                     onDelete
                 }) => {

    const commentAuthor = comment.author;
    const currentUser = useAppStore(state => state.currentUser);
    const [isEditing, setIsEditing] = useState(false);
    const [editedContent, setEditedContent] = useState(comment.content);

    const handleEdit = () => {
        setIsEditing(true);
        setEditedContent(comment.content);
    }

    const handleSave = async () => {
        try {
            const {data} = await $axios.put(`comment/update-comment/${comment.id}`, {
                content: editedContent
            });

            if(data.isSucceed) {
                setIsEditing(false);
                onEdit(comment, editedContent);
            }

        } catch (error) {
            console.log(error);
        }
    }

    return (
        <div className="flex p-4 border-b dark:border-gray-600 text-sm">
            <div className="flex-shrink-0 mr-3">
                <img
                    className="w-10 h-10 rounded-full bg-gray-200"
                    src={commentAuthor.profilePictureUrl}
                    alt={commentAuthor.username}
                />
            </div>
            <div className="flex-1">
                <div className="flex items-center mb-1">
                    <span
                        className="font-bold mr-1 text-xs truncate"
                    >
                        {commentAuthor ? `@${commentAuthor.username}` : "anonymous commentAuthor"}
                    </span>
                    <span
                        className="text-gray-500 text-xs"
                    >
                       {moment(comment.createdAt).fromNow()}
                    </span>
                </div>
                {
                    isEditing ? (
                        <>
                            <Textarea
                                className="mb-2"
                                value={editedContent}
                                onChange={(e) => setEditedContent(e.target.value)}
                            />
                            <div className="flex justify-end gap-2 text-xs">
                                <Button
                                    type="button"
                                    size="sm"
                                    gradientDuoTone="purpleToBlue"
                                    onClick={handleSave}
                                >
                                    Save
                                </Button>
                                <Button
                                    type="button"
                                    size="sm"
                                    gradientDuoTone="purpleToBlue"
                                    outline
                                    onClick={() => setIsEditing(false)}
                                >
                                    Cancell
                                </Button>
                            </div>
                        </>
                    ) : (
                        <>
                            <p className="text-gray-500 mb-2">{comment.content}</p>
                            <div
                                className="flex items-center pt-2 text-xs border-t dark:border-gray-700 max-w-fit gap-2">
                                <button
                                    type="button"
                                    onClick={() => onLike(comment.id)}
                                    className={`text-gray-400 hover:text-blue-500 ${currentUser
                                    && comment.likes.some(like => like.userId === currentUser.id) && "!text-blue-500"}`}>
                                    <FaThumbsUp/>
                                </button>
                                <p
                                    className="text-gray-400"
                                >
                                    {comment.countOfLikes > 0 && comment.countOfLikes + " " + (
                                        comment.countOfLikes === 1 ? "like" : "likes"
                                    )}
                                </p>
                                {
                                    currentUser && (currentUser.id === commentAuthor.id || currentUser.role === ADMIN) && (
                                        <>
                                            <button
                                                type="button"
                                                className="text-gray-400 hover:text-blue-500"
                                                onClick={handleEdit}
                                            >
                                                Edit
                                            </button>
                                            <button
                                                type="button"
                                                className="text-gray-400 hover:text-red-500"
                                                onClick={() => onDelete(comment.id)}
                                            >
                                                Delete
                                            </button>
                                        </>
                                    )
                                }
                            </div>
                        </>
                    )
                }
            </div>
        </div>
    );
};

export default Comment;