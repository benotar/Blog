import moment from "moment";

const Comment = ({key, comment}) => {

    const user = comment.author;

    return(
        <div className="flex p-4 border-b dark:border-gray-600 text-sm">
            <div className="flex-shrink-0 mr-3">
                <img
                    className="w-10 h-10 rounded-full bg-gray-200"
                    src={user.profilePictureUrl}
                    alt={user.username}
                />
            </div>
            <div className="flex-1">
                <div className="flex items-center mb-1">
                    <span
                        className="font-bold mr-1 text-xs truncate"
                    >
                        {user ? `@${user.username}` : "anonymous user"}
                    </span>
                    <span
                        className="text-gray-500 text-xs"
                    >
                       {moment(comment.createdAt).fromNow()}
                    </span>
                </div>
                <p className="text-gray-500 mb-2">{comment.content}</p>
            </div>
        </div>
    );
};

export default Comment;